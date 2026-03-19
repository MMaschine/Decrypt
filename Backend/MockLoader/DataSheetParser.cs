using Decrypt.DataAccess.Entities;
using Esprima;
using Esprima.Ast;
using FluentResults;
using MockLoader.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MockLoader.Helpers;


namespace MockLoader
{
    internal class DataSheetParser
    {
        private const string OrganizationsArrayName = "organizations";
        private const string ProjectsArrayName = "projects";
        private const string UsersArrayName = "users";
        private const string InvoicesArray = "invoices";
        private const string TimeEntryArray = "timeEntries";

        private readonly JsAstHelper _astHelper = new JsAstHelper();

        private Dictionary<string, ArrayExpression> DataArrays { get; set; } = [];

        /// <summary>
        /// Read js file content and extract top layers arrays to the internal dictionary 
        /// </summary>
        /// <param name="data">JS file content</param>
        /// <returns></returns>
        public Result ReadDataArrays(string data)
        {
            try
            {
                var scriptResult = GetScript(data);
                if (scriptResult.IsFailed)
                {
                    return Result.Fail("Failed to handle JS data file");
                }

                var script = scriptResult.Value;

                var arraysResult = _astHelper.BuildTopLevelArraysMap(script);

                DataArrays = arraysResult.Arrays;

                if (!arraysResult.IsSuccess)
                {
                    return Result.Fail(arraysResult.Issues);
                }

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail($"Can't read JS file. Exception: {e.Message}");
            }
        }

        public ParseResult<ParsedOrganization> GetOrganizations() =>
            ParseEntities<ParsedOrganization>(OrganizationsArrayName, MapOrganization);

        public ParseResult<ParsedProject> GetProjects() =>
            ParseEntities<ParsedProject>(ProjectsArrayName, MapProject);

        public ParseResult<ParsedUser> GetUsers() =>
            ParseEntities<ParsedUser>(UsersArrayName, MapUser);

        public ParseResult<ParsedInvoice> GetInvoices() =>
            ParseEntities<ParsedInvoice>(InvoicesArray, MapInvoice);

        public ParseResult<ParsedTimeEntry> GetTimeEntries() =>
            ParseEntities<ParsedTimeEntry>(TimeEntryArray, MapTimeEntry);

        private ParseResult<T> ParseEntities<T>(
            string arrayName,
            Func< ObjectExpression, int, Result<T>> mapEntity)
            where T : class
        {
            var result = new ParseResult<T>();


            if (!DataArrays.TryGetValue(arrayName, out var dataArray))
            {
                result.Issues.Add($"Top-level '{arrayName}' array was not found.");
                return result;
            }
            
            for (var i = 0; i < dataArray.Elements.Count; i++)
            {
                var element = dataArray.Elements[i];

                if (element is not ObjectExpression entityObject)
                {
                    result.Issues.Add($"{arrayName}[{i}] is not an object literal.");
                    continue;
                }

                var mappingResult = mapEntity(entityObject, i);
                if (mappingResult.IsFailed)
                {
                    result.Issues.Add(mappingResult.GetMessage());
                }
                else
                {
                    result.Items.Add(mappingResult.Value);
                }
            }

            return result;
        }

        private Result<ParsedOrganization> MapOrganization(ObjectExpression orgObject, int index)
        {

            if (!_astHelper.TryGetString(orgObject, "id", out var legacyId) ||
                string.IsNullOrWhiteSpace(legacyId))
            {
                return Result.Fail($"organizations[{index}] missing or invalid 'id'.");
            }

            if (!_astHelper.TryGetString(orgObject, "name", out var name) ||
                string.IsNullOrWhiteSpace(name))
            {
                return Result.Fail($"Organization '{legacyId}' missing or invalid 'name'.");
            }

            if (_astHelper.TryGetString(orgObject, "slug", out var slug) || string.IsNullOrEmpty(slug))
            {
                slug = CreateSlug(name);
            }

            if (!_astHelper.TryGetString(orgObject, "contactEmail", out var contactEmail) ||
                string.IsNullOrWhiteSpace(contactEmail))
            {
                return Result.Fail($"Organization '{legacyId}' missing or invalid 'contactEmail'.");
            }

            if (!_astHelper.TryGetString(orgObject, "tier", out var tier) ||
                string.IsNullOrWhiteSpace(tier))
            {
                return Result.Fail($"Organization '{legacyId}' missing or invalid 'tier'.");
            }

            if (!_astHelper.TryGetString(orgObject, "industry", out var industry) ||
                string.IsNullOrWhiteSpace(industry))
            {
                return Result.Fail($"Organization '{legacyId}' missing or invalid 'industry'.");
            }

            _astHelper.TryGetDateTime(orgObject, "createdAt", out var createdAt);
            _astHelper.TryGetString(orgObject, "description", out var description);
            _astHelper.TryGetNestedString(orgObject, "settings", "timezone", out var timezone);
            _astHelper.TryGetNestedString(orgObject, "settings", "currency", out var currency);
            _astHelper.TryGetNestedBool(orgObject, "settings", "allowOvertime", out var allowOvertime);
            _astHelper.TryGetNestedString(orgObject, "settings", "defaultLocale", out var defaultLocale);
            _astHelper.TryGetNestedString(orgObject, "metadata", "source", out var source);
            _astHelper.TryGetNestedDateTime(orgObject, "metadata", "migratedAt", out var migratedAt);
            _astHelper.TryGetNestedInt(orgObject, "metadata", "legacyId", out var metadataLegacyId);

            var organization = new Organization
            {
                LegacyId = legacyId,
                Name = name,
                Slug = slug,
                ContactEmail = contactEmail,
                Industry = industry,
                Tier = tier,
                CreatedAt = createdAt,
                Description = description,
                AllowOvertime = allowOvertime,
                Timezone = timezone,
                DefaultLocale = defaultLocale,
                Metadata = 
                {
                    LegacyNumericId = metadataLegacyId,
                    Source = source,
                    MigratedAt = migratedAt
                }
            };


            return Result.Ok(new ParsedOrganization()
            {
                SourceIndex = index,
                Entity = organization,
                CurrencyCode = currency
            });
        }

        private Result<ParsedProject> MapProject(ObjectExpression projectObject, int index)
        {
            if (!_astHelper.TryGetString(projectObject, "id", out var legacyId) ||
                string.IsNullOrWhiteSpace(legacyId))
            {
                return Result.Fail($"projects[{index}] missing or invalid 'id'.");
            }

            if (!_astHelper.TryGetString(projectObject, "orgId", out var organizationLegacyId) ||
                string.IsNullOrWhiteSpace(organizationLegacyId))
            {
                return Result.Fail($"Project '{legacyId}' missing or invalid 'organizationId'.");
            }

            if (!_astHelper.TryGetString(projectObject, "name", out var name) ||
                string.IsNullOrWhiteSpace(name))
            {
                return Result.Fail($"Project '{legacyId}' missing or invalid 'name'.");
            }

            if (!_astHelper.TryGetString(projectObject, "status", out var status) ||
                string.IsNullOrWhiteSpace(status))
            {
                return Result.Fail($"Project '{legacyId}' missing or invalid 'status'.");
            }

            _astHelper.TryGetInt(projectObject, "budgetHours", out var budgetHours);
            _astHelper.TryGetString(projectObject, "description", out var description);

            DateOnly? startDate = null;
            if (_astHelper.TryGetDateTime(projectObject, "startDate", out var parsedStartDate))
            {
                startDate = DateOnly.FromDateTime(parsedStartDate);
            }

            DateOnly? endDate = null;
            if (_astHelper.TryGetDateTime(projectObject, "endDate", out var parsedEndDate))
            {
                endDate = DateOnly.FromDateTime(parsedEndDate);
            }

            var project = new Project
            {
                LegacyId = legacyId,
                Name = name,
                Status = status,
                BudgetHours = budgetHours ?? 0,
                StartDate = startDate,
                EndDate = endDate,
                Description = description
            };

            return Result.Ok(new ParsedProject
            {
                SourceIndex = index,
                Entity = project,
                OrganizationLegacyId = organizationLegacyId
            });
        }

        private Result<ParsedUser> MapUser(ObjectExpression userObject, int index)
        {
            if (!_astHelper.TryGetString(userObject, "id", out var legacyId) ||
                string.IsNullOrWhiteSpace(legacyId))
            {
                return Result.Fail($"users[{index}] missing or invalid 'id'.");
            }

            if (!_astHelper.TryGetString(userObject, "orgId", out var organizationLegacyId) ||
                string.IsNullOrWhiteSpace(organizationLegacyId))
            {
                return Result.Fail($"User '{legacyId}' missing or invalid 'organizationId'.");
            }

            if (!_astHelper.TryGetString(userObject, "name", out var name) ||
                string.IsNullOrWhiteSpace(name))
            {
                return Result.Fail($"User '{legacyId}' missing or invalid 'name'.");
            }

            if (!_astHelper.TryGetString(userObject, "email", out var email) ||
                string.IsNullOrWhiteSpace(email))
            {
                return Result.Fail($"User '{legacyId}' missing or invalid 'email'.");
            }

            if (!_astHelper.TryGetString(userObject, "role", out var userRole) ||
                string.IsNullOrWhiteSpace(userRole))
            {
                return Result.Fail($"User '{legacyId}' missing or invalid 'role'.");
            }

            _astHelper.TryGetBool(userObject, "isActive", out var isActive);
            _astHelper.TryGetString(userObject, "bio", out var bio);

            var user = new User
            {
                LegacyId = legacyId,
                Name = name,
                Email = email,
                UserRole = userRole,
                IsActive = isActive,
                Bio = bio
            };

            return Result.Ok(new ParsedUser
            {
                SourceIndex = index,
                Entity = user,
                OrganizationLegacyId = organizationLegacyId
            });
        }

        private Result<ParsedInvoice> MapInvoice(ObjectExpression invoiceObject, int index)
        {
            if (!_astHelper.TryGetString(invoiceObject, "id", out var legacyId) ||
                string.IsNullOrWhiteSpace(legacyId))
            {
                return Result.Fail($"invoices[{index}] missing or invalid 'id'.");
            }

            if (!_astHelper.TryGetString(invoiceObject, "orgId", out var organizationLegacyId) ||
                string.IsNullOrWhiteSpace(organizationLegacyId))
            {
                return Result.Fail($"Invoice '{legacyId}' missing or invalid 'organizationId'.");
            }

            if (!_astHelper.TryGetString(invoiceObject, "projectId", out var projectLegacyId) ||
                string.IsNullOrWhiteSpace(projectLegacyId))
            {
                return Result.Fail($"Invoice '{legacyId}' missing or invalid 'projectId'.");
            }

            if (!_astHelper.TryGetString(invoiceObject, "currency", out var currencyCode) ||
                string.IsNullOrWhiteSpace(currencyCode))
            {
                return Result.Fail($"Invoice '{legacyId}' missing or invalid 'currency'.");
            }

            if (!_astHelper.TryGetString(invoiceObject, "status", out var status) ||
                string.IsNullOrWhiteSpace(status))
            {
                return Result.Fail($"Invoice '{legacyId}' missing or invalid 'status'.");
            }

            if (!_astHelper.TryGetDecimal(invoiceObject, "amount", out var amount) ||
                amount is null)
            {
                return Result.Fail($"Invoice '{legacyId}' missing or invalid 'amount'.");
            }

            _astHelper.TryGetString(invoiceObject, "description", out var description);

            DateOnly? dueDate = null;
            if (_astHelper.TryGetDateTime(invoiceObject, "dueDate", out var parsedDueDate))
            {
                dueDate = DateOnly.FromDateTime(parsedDueDate);
            }

            DateOnly? issuedAt = null;
            if (_astHelper.TryGetDateTime(invoiceObject, "issuedAt", out var parsedIssuedAt))
            {
                issuedAt = DateOnly.FromDateTime(parsedIssuedAt);
            }

            var invoice = new Invoice
            {
                LegacyId = legacyId,
                Amount = amount.Value,
                Status = status,
                DueDate = dueDate,
                IssuedAt = issuedAt,
                Description = description
            };

            return Result.Ok(new ParsedInvoice
            {
                SourceIndex = index,
                Entity = invoice,
                OrganizationLegacyId = organizationLegacyId,
                ProjectLegacyId = projectLegacyId,
                CurrencyCode = currencyCode
            });
        }

        private Result<ParsedTimeEntry> MapTimeEntry(
            ObjectExpression timeEntryObject,
            int index)
        {
            if (!_astHelper.TryGetString(timeEntryObject, "id", out var legacyId) ||
                string.IsNullOrWhiteSpace(legacyId))
            {
                return Result.Fail($"timeEntries[{index}] missing or invalid 'id'.");
            }

            if (!_astHelper.TryGetString(timeEntryObject, "userId", out var userLegacyId) ||
                string.IsNullOrWhiteSpace(userLegacyId))
            {
                return Result.Fail($"TimeEntry '{legacyId}' missing or invalid 'userId'.");
            }

            if (!_astHelper.TryGetString(timeEntryObject, "projectId", out var projectLegacyId) ||
                string.IsNullOrWhiteSpace(projectLegacyId))
            {
                return Result.Fail($"TimeEntry '{legacyId}' missing or invalid 'projectId'.");
            }

            if (!_astHelper.TryGetDateTime(timeEntryObject, "date", out var parsedDate))
            {
                return Result.Fail($"TimeEntry '{legacyId}' missing or invalid 'date'.");
            }

            if (!_astHelper.TryGetInt(timeEntryObject, "hours", out var hours))
            {
                return Result.Fail($"TimeEntry '{legacyId}' missing or invalid 'hours'.");
            }

            _astHelper.TryGetString(timeEntryObject, "description", out var description);

            var timeEntry = new TimeEntry
            {
                LegacyId = legacyId,
                Date = DateOnly.FromDateTime(parsedDate),
                Hours = hours ?? 0, //TODO: Validate that it is allowed and update validation 
                Description = description
            };

            return Result.Ok(new ParsedTimeEntry
            {
                SourceIndex = index,
                Entity = timeEntry,
                UserLegacyId = userLegacyId,
                ProjectLegacyId = projectLegacyId
            });
        }

        private Result<Script> GetScript(string jsCode)
        {
            if (string.IsNullOrWhiteSpace(jsCode))
            {
                return Result.Fail("JS content is empty.");
            }

            try
            {
                var parser = new JavaScriptParser(new ParserOptions
                {
                    Tolerant = true
                });

                return parser.ParseScript(jsCode);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Failed to parse JS file: {ex.Message}");
            }
        }

        private string CreateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.ToLowerInvariant();

            // Remove accents (é → e, ü → u, etc.)
            value = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in value)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            value = sb.ToString().Normalize(NormalizationForm.FormC);

            // Replace anything not a-z or digit with dash
            value = Regex.Replace(value, @"[^a-z0-9]+", "-");

            // Trim dashes from start/end
            return value.Trim('-');
        }
    }

}
