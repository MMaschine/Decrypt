using Decrypt.DataAccess.Entities;
using Decrypt.DataAccess.Entities.References;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using MockLoader.DbInfrastructure;
using MockLoader.Models;
using System.Collections.Generic;

namespace MockLoader
{
    internal class DataExtractor(DbContextWrapper wrapper)
    {
        private readonly DataSheetParser _parser = new();

        /// <summary>
        /// Extracts data from the file and save it to the DB 
        /// </summary>
        /// <param name="data">Content of the js file as string</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Data must not be empty</exception>
        public async Task<Result> PopulateDataBaseAsync(string data)
        {
            #region Validation

            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            var dbConnectionResult = await wrapper.TryConnectionAsync();

            if (dbConnectionResult.IsFailed)
            {
                return Result.Fail("Db is not accessible");
            }

            #endregion

            try
            {
                //Raad JS file and get arrays from it 
                var arraysResult = _parser.ReadDataArrays(data);

                if (arraysResult.IsFailed)
                {
                    LogErrors(arraysResult);
                    return Result.Fail("Data extraction failed, can't get arrays with entities");
                }

                //Mapping of entities 
                var organizationsTask = Task.Run(() => SafeExecute(() => _parser.GetOrganizations(), "organizations"));
                var projectsTask = Task.Run(() => SafeExecute(() => _parser.GetProjects(), "projects"));
                var invoicesTask = Task.Run(() => SafeExecute(() => _parser.GetInvoices(), "invoices"));
                var usersTask = Task.Run(() => SafeExecute(() => _parser.GetUsers(), "users"));
                var timeEntriesTask = Task.Run(() => SafeExecute(() => _parser.GetTimeEntries(), "timeEntries"));

                await Task.WhenAll(
                    organizationsTask,
                    projectsTask,
                    invoicesTask,
                    usersTask,
                    timeEntriesTask);

                var result = new DataExtractionResult();
                var issues = new List<string>();

                await MergeAsync(organizationsTask, result.Organizations, issues);
                await MergeAsync(projectsTask, result.Projects, issues);
                await MergeAsync(invoicesTask, result.Invoices, issues);
                await MergeAsync(usersTask, result.Users, issues);
                await MergeAsync(timeEntriesTask, result.TimeEntries, issues);

                if (issues.Any())
                {
                    Console.WriteLine("Parsing issues:");
                    LogErrors(issues);
                }

                Console.WriteLine("Parsing complete");

                await SaveDataAsync(result);

                Console.WriteLine("Data exported to the DB");
            }
            catch (Exception e)
            {
                return Result.Fail($"Failed to save data. Message: {e.Message}");
            }

            return Result.Ok();
        }

        private async Task SaveDataAsync(DataExtractionResult extractionResult)
        {
            //Reference can be updated separately from parsing, so need to be sure that we have proper list of currencies 
            var currencyCodes = extractionResult.Organizations
                .Select(x => x.CurrencyCode)
                .Union(extractionResult.Invoices.Select(x => x.CurrencyCode))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();


            await UpdateCurrencyReferenceAsync(currencyCodes.Select(x => new Currency { Code = x, Name = x }).ToArray());

            var currencyByCode = (await wrapper.GetDataSource<Currency>()
                .GetAllAsync())
                .ToDictionary(x=>x.Code);

            var issues = new List<string>();

            var organizations = new List<Organization>();
            var legacyOrganizations = new Dictionary<string, Organization>();

            foreach (var item in extractionResult.Organizations)
            {
                var entity = item.Entity;

                if (!legacyOrganizations.TryAdd(entity.LegacyId, entity))
                {
                    issues.Add($"Duplicate organization legacy id '{entity.LegacyId}'.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.CurrencyCode))
                {
                    issues.Add($"Organization '{entity.LegacyId}' has empty currency code.");
                    legacyOrganizations.Remove(entity.LegacyId);
                    continue;
                }

                if (!currencyByCode.TryGetValue(item.CurrencyCode, out var currency))
                {
                    issues.Add($"Currency '{item.CurrencyCode}' was not found for organization '{entity.LegacyId}'.");
                    legacyOrganizations.Remove(entity.LegacyId);
                    continue;
                }

                entity.CurrencyId = currency.Id;
                organizations.Add(entity);
            }

            var users = new List<User>();
            var legacyUsers = new Dictionary<string, User>();

            foreach (var item in extractionResult.Users)
            {
                var entity = item.Entity;

                if (!legacyUsers.TryAdd(entity.LegacyId, entity))
                {
                    issues.Add($"Duplicate user legacy id '{entity.LegacyId}'.");
                    continue;
                }

                if (!legacyOrganizations.TryGetValue(item.OrganizationLegacyId, out var organization))
                {
                    issues.Add($"Organization '{item.OrganizationLegacyId}' was not found for user '{entity.LegacyId}'.");
                    legacyUsers.Remove(entity.LegacyId);
                    continue;
                }

                entity.Organization = organization;
                users.Add(entity);
            }

            var projects = new List<Project>();
            var legacyProjects = new Dictionary<string, Project>();

            foreach (var item in extractionResult.Projects)
            {
                var entity = item.Entity;

                if (!legacyProjects.TryAdd(entity.LegacyId, entity))
                {
                    issues.Add($"Duplicate project legacy id '{entity.LegacyId}'.");
                    continue;
                }

                if (!legacyOrganizations.TryGetValue(item.OrganizationLegacyId, out var organization))
                {
                    issues.Add($"Organization '{item.OrganizationLegacyId}' was not found for project '{entity.LegacyId}'.");
                    legacyProjects.Remove(entity.LegacyId);
                    continue;
                }

                entity.Organization = organization;
                projects.Add(entity);
            }

            var invoices = new List<Invoice>();

            foreach (var item in extractionResult.Invoices)
            {
                var entity = item.Entity;

                if (!legacyOrganizations.TryGetValue(item.OrganizationLegacyId, out var organization))
                {
                    issues.Add($"Organization '{item.OrganizationLegacyId}' was not found for invoice '{entity.LegacyId}'.");
                    continue;
                }

                if (!legacyProjects.TryGetValue(item.ProjectLegacyId, out var project))
                {
                    issues.Add($"Project '{item.ProjectLegacyId}' was not found for invoice '{entity.LegacyId}'.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.CurrencyCode))
                {
                    issues.Add($"Invoice '{entity.LegacyId}' has empty currency code.");
                    continue;
                }

                if (!currencyByCode.TryGetValue(item.CurrencyCode, out var currency))
                {
                    issues.Add($"Currency '{item.CurrencyCode}' was not found for invoice '{entity.LegacyId}'.");
                    continue;
                }

                entity.Organization = organization;
                entity.Project = project;
                entity.CurrencyId = currency.Id;

                invoices.Add(entity);
            }

            var timeEntries = new List<TimeEntry>();

            foreach (var item in extractionResult.TimeEntries)
            {
                var entity = item.Entity;

                if (!legacyProjects.TryGetValue(item.ProjectLegacyId, out var project))
                {
                    issues.Add($"Project '{item.ProjectLegacyId}' was not found for time entry '{entity.LegacyId}'.");
                    continue;
                }

                if (!legacyUsers.TryGetValue(item.UserLegacyId, out var user))
                {
                    issues.Add($"User '{item.UserLegacyId}' was not found for time entry '{entity.LegacyId}'.");
                    continue;
                }

                entity.Project = project;
                entity.User = user;

                timeEntries.Add(entity);
            }

            wrapper.Context.Organizations.AddRange(organizations);
            wrapper.Context.Users.AddRange(users);
            wrapper.Context.Projects.AddRange(projects);
            wrapper.Context.Invoices.AddRange(invoices);
            wrapper.Context.TimeEntries.AddRange(timeEntries);

            try
            {
                await wrapper.Context.SaveChangesAsync();
                Console.WriteLine("Data saved");
            }
            catch (Exception ex)
            {
                issues.Add($"Database save failed: {ex.Message}");
            }

            if (issues.Any())
            {
                Console.WriteLine("Data consistency violations found: ");
                LogErrors(issues);
            }
        }

        private async Task<int> UpdateCurrencyReferenceAsync(Currency[] currencySet)
        {
            if (currencySet.Length == 0)
                return 0;

            var currencySource = wrapper.GetDataSource<Currency>();

            var inputCodes = currencySet
                .Select(x => x.Code)
                .ToArray();

            var existingCodeSet = (await currencySource.GetQueryableItems()
                .Where(x => inputCodes.Contains(x.Code))
                .Select(x => x.Code)
                .ToArrayAsync()).ToHashSet();

            var entitiesToAdd = currencySet
                .Where(x => !existingCodeSet.Contains(x.Code))
                .ToArray();

            if (entitiesToAdd.Length == 0)
                return 0;

            await currencySource.AddRangeAsync(entitiesToAdd);

            return entitiesToAdd.Length;
        }

        private ParseResult<T> SafeExecute<T>(Func<ParseResult<T>> action, string entityName)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                var failedResult = new ParseResult<T>();
                failedResult.Issues.Add($"Unhandled error while parsing '{entityName}': {ex.Message}");
                return failedResult;
            }
        }

        private  async Task MergeAsync<T>(Task<ParseResult<T>> task, ICollection<T> targetItems, ICollection<string> issues)
        {
            var res = await task;

            if (res.Items.Count > 0)
                foreach (var item in res.Items)
                    targetItems.Add(item);

            if (res.Issues.Count > 0)
                foreach (var issue in res.Issues)
                    issues.Add(issue);
        }

        private void LogErrors(List<string> issues)
        {
            foreach (var issue in issues)
            {
                Console.WriteLine($"Issue found --> {issue}");
            }
        }

        private void LogErrors(Result result)
        {
            foreach (var issue in result.Errors)
            {
                Console.WriteLine($"Issue found --> {issue}");
            }
        }
    }
}
