---
mcg-prompt:
  title: "Start SDLC Journey"
  description: "Launch your development journey with intelligent ticket analysis and workflow automation"
  instruct: "Kickstart the SDLC process by using Jira MCP server for retrieving comprehensive ticket details including requirements, status, assignee, priority, labels, components, comments, attachments, and linked issues. Provide analysis based on the selected focus area with clear sections and bullet points for easy reading."
  order: 1
  fields:
    - id: "ticket_key"
      type: "text"
      label: "Jira Ticket Key"
      placeholder: "e.g., PROJ-123"
      required: true
    - id: "analysis_focus"
      type: "dropdown"
      label: "Analysis Focus"
      options: ["Full Analysis", "Generate Acceptance Criteria", "Generate Design Approach", "Generate Pseudo Code", "Code Checklist", "Generate Tests", "Generate Code", "Generate PR Review", "Review Code", "Create Pull Request"]
      default: "Full Analysis"
      required: true
---

# Read & Analyze Jira Ticket

Analyze ticket **{{ ticket_key }}** with focus on **{{ analysis_focus }}**.

{{#if (eq analysis_focus "Full Analysis")}}
### Full Analysis

- **Summary**: Brief overview of the ticket
- **Technical Complexity**: Assessment of implementation difficulty
- **Business Value**: Impact and importance
- **Dependencies**: Related tickets and blockers
- **Acceptance Criteria**: Current state and completeness
- **Risk Factors**: Potential issues or concerns
- **Recommendations**: Next steps and suggestions
{{/if}}

{{#if (eq analysis_focus "Generate Acceptance Criteria")}}
### Generate Acceptance Criteria

**Role-Based Prompt Structure for Requirement Analyst**

---

**Role:** Requirement Analyst

**Objective:** Break down a high-level requirement into smaller, actionable user stories

---

**Prompt Structure:**

1. **High-Level Requirement:**  
  **Instructions:**
   - [Use the requirements fetched from Description field of Jira Story pulled from JIRA MCP server]

2. **Stakeholders & User Roles:**  
   - Who are the primary users or stakeholders?
   - What roles interact with this feature?

3. **Business Goals:**  
   - What business value does this requirement deliver?
   - What problems does it solve?

4. **Breakdown into User Stories:**  
   For each user role, create user stories using the format:  
   - As a [user role], I want [functionality] so that [business value].

5. **Acceptance Criteria for Each Story:**  
   - List clear, testable conditions for each story.

6. **Open Questions/Risks:**  
   - List any uncertainties, assumptions, or potential risks.

**Output Format:**

- **Summary**: Brief overview of the ticket
- **Technical Complexity**: Assessment of implementation difficulty  
- **Business Value**: Impact and importance
- **Dependencies**: Related tickets and blockers
- **Acceptance Criteria**: Current state and completeness
- **Risk Factors**: Potential issues or concerns
- **Recommendations**: Next steps and suggestions
{{/if}}

{{#if (eq analysis_focus "Generate Design Approach")}}
### Generate Design Approach

**Role-Based Prompt: Requirement to Implementation Approach**

---

**Role:** Solution Architect / Senior Developer

**Objective:** Take a new requirement, review the existing codebase, and suggest an implementation approach that aligns with current architecture and best practices

---

**Prompt Structure:**

1. **Requirement/Acceptance Criteria Input:**  
   **Instructions:**
   [Use the requirements fetched from Description field of Jira Story pulled from JIRA MCP server]
   [Use the Acceptance Criteria from **{{ ticket_key }}**-Analysis md file or **{{ ticket_key }}** Analysis md file in analysis folder under mcg-prompts folder]
   Consider the following:
   - Current architecture and design patterns are from Chimera Architecture
   - Existing codebase structure and components
   - Make sure healthcare standards and compliance are considered with respect to PAS API and FHIR standards

2. **Codebase Review Checklist:**  
   - Which existing modules, services, or components are relevant to this requirement?
   - Are there reusable models, controllers, or views?
   - Are there any architectural patterns or constraints to consider?
   - What dependencies or integrations exist that may impact implementation?

3. **Suggested Implementation Approach:**  
   - High-level steps to implement the requirement for each user story.
   - Which files or layers will need to be created or modified?
   - Any recommended patterns (e.g., service, repository, MVC)?
   - How to ensure maintainability and testability?
   - Any potential risks or considerations?
   - Don't over complicate the solution for future maintenance.
   - Use best practices that are extensible

4. **Open Questions:**  
   - Any clarifications needed from stakeholders or product owners?
   - Any assumptions made during the review?

---

**Instructions:**  

- Fill in each section based on the requirement and your review of the codebase.
- Be concise and specific in your recommendations.
{{/if}}

{{#if (eq analysis_focus "Generate Pseudo Code")}}
### Generate Pseudo Code

**Task-Based Developer Prompt**

---

**Objective:**  
Provide clear, actionable steps for a developer to implement a requirement efficiently.

---

**Prompt Structure:**

1. **Acceptance Criteria Overview:**  
  **Instructions:**
   [Use the requirements fetched from Description field of Jira Story pulled from JIRA MCP server]
   [Use the Acceptance Criteria from **{{ ticket_key }}**-Analysis md file or **{{ ticket_key }}** Analysis md file in analysis folder under mcg-prompts folder]
   [Use the Design approach from **{{ ticket_key }}** Design approach md file in analysis folder under mcg-prompts folder]

2. **Step-by-Step Tasks:**  
   - Create pseudo code
   - List each development step as a checklist.
   - Include file names and locations where applicable.
   - Specify any new classes, methods, or views to be created or modified.
   - Ensure the order follows the overall design and architecture

3. **Acceptance Criteria:**  
   - Restate the testable outcomes that must be met for the task to be considered complete.

4. **Testing Instructions:**  
   - Outline how to manually or automatically test the feature.

5. **Notes/Considerations:**  
   - Mention any coding standards, patterns, or best practices to follow.
   - Highlight any known risks or dependencies.
   - Add required Tests for each step
   - Do not write any code at this point

---

**Instructions:**  

- Work through each checklist item in order.
- Mark each step as complete before moving to the next.
- Communicate any blockers or questions early.
{{/if}}

{{#if (eq analysis_focus "Code Checklist")}}
### Code Checklist

- **Architecture Review**: Design patterns, SOLID principles, scalability
- **Code Quality**: Code standards, naming conventions, documentation
- **Security Checklist**: OWASP guidelines, healthcare data protection
- **Performance**: Query optimization, caching strategies, load handling
- **Testing Coverage**: Unit tests, integration tests, end-to-end tests
- **Healthcare Standards**: HL7 FHIR compliance, medical terminology
- **Error Handling**: Logging, monitoring, graceful degradation
- **Documentation**: API docs, technical specifications, deployment guides
{{/if}}

{{#if (eq analysis_focus "Generate Tests")}}
### Generate Tests

**Output Format:**

- **Unit Test Scenarios**: Individual component testing strategies
- **Integration Test Cases**: System interaction and data flow testing
- **End-to-End Test Scripts**: Complete user workflow validation
- **Performance Test Plans**: Load testing, stress testing, scalability
- **Security Test Cases**: Penetration testing, vulnerability assessment
- **Healthcare Compliance Testing**: HIPAA compliance, data privacy validation
- **Regression Test Suite**: Existing functionality preservation
- **Test Data Management**: Mock data, test environments, data privacy
- **Follow Existing Test Framework**: Ensure compatibility with current testing tools and practices

**Instructions:**
[Use the requirements fetched from Description field of Jira Story pulled from JIRA MCP server]
[Use the Pseudo Code implementation from **{{ ticket_key }}** Pseudo Code md file in analysis folder under mcg-prompts folder]

**Output Format:**

- Implement the solution based on the pseudo code
- Follow the design approach and architecture patterns
- Include proper error handling and logging
- Add appropriate unit tests
- Ensure compliance with healthcare standards
{{/if}}

{{#if (eq analysis_focus "Generate PR Review")}}
### Generate PR Review

**Output Format:**

- Fill in the below PR template with required information

**Output Format:**

### Description


## Jira ticket(s)
Reference any related issues.

- [DVT-1234](https://mcghealth.atlassian.net/browse/DVT-1234)

### **Why**
_The reason for making these changes._

### **What**
_A concise description of the changes._

### **How**
_An overview of how the changes were implemented._



## Checklist
Ensure all tasks are completed before requesting a review.

- [ ] Reviewed Sonar results and the gates are passing, including coverage
- [ ] Reviewed Snyk report and no new vulnerabilities introduced.
- [ ] Code follows the coding standards and best practices.
- [ ] Code is well documented.

## Types of Changes
Identify the nature of your changes.

- [ ] New feature
- [ ] Bug fix
- [ ] Breaking change
- [ ] Documentation update
- [ ] Other (please describe)

## Detailed Changes
List and describe the specific changes made in this pull request.


## Screenshots (if applicable)
Include any screenshots or visual documentation relevant to the changes.

![Screenshot Description](screenshot-url)
 
## Documentation
Indicate if documentation has been updated or created as a result of this change.

- [ ] Documentation Updated
- [ ] Documentation Not Required


## Additional Notes
Include any additional information or context that may be helpful for the reviewers.

---

**Output Format:**

- **Code Quality Assessment**: Readability, maintainability, best practices
- **Architecture Compliance**: Design pattern adherence, system integration
- **Security Review**: Vulnerability assessment, healthcare data protection
- **Performance Impact**: Code efficiency, database optimization, caching
- **Testing Validation**: Test coverage, test quality, edge case handling
- **Healthcare Standards**: FHIR compliance, medical data handling standards
- **Documentation Review**: Code comments, API documentation, technical specs
- **Deployment Readiness**: Configuration management, environment compatibility
{{/if}}


{{#if (eq analysis_focus "Review Code")}}
# Code Review Instructions

This is a prompt for you (the LLM) to perform an actual code review. Don't create another prompt file or template - instead, use this framework to actually analyze the code in the Current PR or branch and provide a detailed review of code changes.

## Role-Based Code Review

---

**Role:** Senior Software Architect specializing in C# and Chimera Architecture

**Objective:** Perform a comprehensive, structured code review of the current PR/branch that ensures high-quality, maintainable code adhering to architectural standards and best practices while providing actionable feedback.

---

## Instructions

Begin by gathering all necessary context:

1. Retrieve and review the requirements from the Description field of Jira Story {TICKET_KEY}
2. Examine the actual code changes in the current branch/PR
3. Reference the PR review documentation if available
4. Focus specifically on the code changes made in this PR, not the entire codebase
5. Examine the code for compliance with Chimera Architecture, C# best practices, and healthcare standards (FHIR, PAS API)
6. Use the provided checklist to evaluate the code against established criteria
7. Thoroughly evaluate the code to identify potential issues or areas for enhancement. Avoid approving changes as-is, as there is always room for refinement."

## Review Process

1. For each section of the checklist below, review the actual code and assess whether it meets the criteria
2. Mark fulfilled items with a checkmark (✓) and include examples from the code showing compliance
3. Mark unfulfilled items with an X (✗), explain why, and provide specific recommendations for improvement
4. Mark items as not applicable (N/A) where appropriate with a brief explanation
5. Provide specific examples from the code to support your assessment
6. Include concrete, actionable recommendations for any improvements needed

## Review Checklist

Evaluate the actual code changes against each of the following categories. For each checklist item:
- Mark fulfilled items with a checkmark (✓)
- Mark unfulfilled items with an X (✗) and provide a reason why
- Mark items as not applicable (N/A) where appropriate
- Provide specific examples from the code to support your assessment
- Include concrete, actionable recommendations for any improvements needed

### 🔍 **Code Quality**

- [ ] Code follows established naming conventions
    - Example: `GetPatientInfo()` instead of `getinfo` or `gpinfo`.
- [ ] Methods and classes have clear, single responsibilities
    - Example: `PatientValidator` only validates patient data, not processes it.
- [ ] No code duplication or redundant operations
    - Example: Extract repeated logic into a helper method, e.g., `FormatErrorMessage()`.
- [ ] Appropriate use of design patterns
    - Example: Use Factory pattern for creating error responses: `ValidationExceptionFactory.Create()`.
- [ ] Cyclomatic complexity is reasonable
    - Example: Refactor a method with 10+ if/else branches into smaller methods.
- [ ] No unused variables or dead code
    - Example: Remove `string temp = "";` if not used anywhere.

### 🏥 **Domain-Specific (Healthcare/FHIR)**

- [ ] FHIR standard compliance verified
    - Example: Error responses use `OperationOutcome` resource as per FHIR spec.
- [ ] Healthcare data validation implemented
    - Example: Validate that `Patient.Identifier` is not null and matches expected format.
- [ ] Patient data privacy considerations addressed
    - Example: Ensure no PHI is logged in error messages or logs.
- [ ] Proper handling of medical terminology/codes
    - Example: Use HL7 codes for error outcomes, e.g., `MSG_PARAM_INVALID`.
- [ ] Integration with existing healthcare workflows
    - Example: PR includes integration test for EHR workflow.

### 🚨 **Error Handling**

- [ ] Proper exception types used
    - Example: Throw `FhirValidationException` for FHIR validation errors, not generic `Exception`.
- [ ] Error messages are clear and actionable
    - Example: "Patient reference is missing" instead of "Error occurred".
- [ ] HTTP status codes are appropriate (400 vs 500)
    - Example: Return 400 for client validation errors, 500 for server errors.
- [ ] FHIR-compliant error responses
    - Example: Use `OperationOutcome` with proper issue type and code.
- [ ] Generic error messages for security-sensitive scenarios
    - Example: "Invalid credentials" instead of "User not found in database".
- [ ] Operation outcome codes properly implemented
    - Example: Use `MSG_PARAM_INVALID` for invalid parameter errors.

### 🧪 **Testing**

- [ ] Unit tests cover happy path scenarios
    - Example: Test that valid patient data returns success.
- [ ] Edge cases tested (null, empty, invalid inputs)
    - Example: Test with `null`, `""`, and invalid patient references.
- [ ] Integration tests for end-to-end scenarios
    - Example: Test full API workflow from request to response.
- [ ] Test names are descriptive and meaningful
    - Example: `Should_ReturnError_When_PatientReferenceIsNull()`.
- [ ] TestCase attributes used for parameterized tests
    - Example: `[TestCase(null)]`, `[TestCase("")]`, `[TestCase("invalid")]`.
- [ ] Proper test data organization
    - Example: Use `TestDataBuilder` for creating test objects.
- [ ] Assert statements have clear expectations
    - Example: `Assert.AreEqual("Patient reference is missing", result.ErrorMessage);`
- [ ] Test coverage meets minimum requirements
    - Example: Coverage report shows >90% for new code.

### 📚 **Documentation & Comments**

- [ ] Code comments explain WHY, not just WHAT
    - Example: `// Using OperationOutcome for FHIR compliance`
- [ ] TODO comments linked to JIRA tickets
    - Example: `// TODO: Refactor validation logic (JIRA-1234)`
- [ ] Public methods have XML documentation
    - Example: `/// <summary>Validates patient reference for FHIR compliance</summary>`
- [ ] README updated if needed
    - Example: Add new API endpoint details to README.
- [ ] API documentation updated
    - Example: Update Swagger/OpenAPI docs for new endpoints.
- [ ] Breaking changes documented
    - Example: Add migration notes for removed fields in changelog.

### 🎨 **Code Formatting & Style**

- [ ] Consistent indentation and spacing
    - Example: All code uses 4-space indentation.
- [ ] Proper line breaks between logical sections
    - Example: Add blank line between method definitions.
- [ ] Comments properly positioned (above relevant code)
    - Example: Place summary comment above method, not inline.
- [ ] No trailing whitespace
    - Example: No extra spaces at end of lines.
- [ ] File organization follows project structure
    - Example: Place new service in `Services/` folder.
- [ ] Using statements properly organized
    - Example: System namespaces listed first, sorted alphabetically.

### 🔧 **Performance & Efficiency**

- [ ] No unnecessary resource allocation
    - Example: Avoid creating new objects inside loops unless needed.
- [ ] Proper disposal of IDisposable objects
    - Example: Use `using` statement for database connections.
- [ ] Efficient use of data structures
    - Example: Use `Dictionary` for fast lookups instead of `List`.
- [ ] No redundant API calls or database queries
    - Example: Cache results to avoid duplicate queries.
- [ ] Async/await used appropriately
    - Example: Use `await` for I/O-bound operations, not CPU-bound.
- [ ] Memory leaks prevented
    - Example: Unsubscribe from events in `Dispose()` method.

### 🔐 **Security**

- [ ] Input validation implemented
    - Example: Validate all user input before processing.
- [ ] No sensitive data in logs
    - Example: Do not log patient SSN or medical record numbers.
- [ ] Proper authentication/authorization
    - Example: Use JWT tokens for API authentication.
- [ ] SQL injection prevention
    - Example: Use parameterized queries for all DB access.
- [ ] XSS prevention measures
    - Example: Encode output in web responses.
- [ ] Secure configuration management
    - Example: Store secrets in Azure Key Vault, not in code.

### 🏗️ **Architecture & Design**

- [ ] Changes align with existing architecture
    - Example: New service implements existing interface pattern.
- [ ] Dependency injection used appropriately
    - Example: Register new service in DI container.
- [ ] Separation of concerns maintained
    - Example: Validation logic in validator class, not controller.
- [ ] Interface contracts respected
    - Example: Implement all required interface methods.
- [ ] Configuration externalized properly
    - Example: Use `appsettings.json` for config, not hardcoded values.
- [ ] Logging implemented consistently
    - Example: Use shared logger for all error reporting.

### 🔄 **Integration & Deployment**

- [ ] Pipeline builds successfully
    - Example: Azure DevOps pipeline shows green for all stages.
- [ ] All tests pass
    - Example: All unit and integration tests pass in CI.
- [ ] Code coverage maintained or improved
    - Example: Coverage report shows no decrease after PR merge.
- [ ] No breaking changes to existing APIs
    - Example: Existing clients continue to work after deployment.
- [ ] Database migrations if needed
    - Example: Migration script included for new table changes.
- [ ] Configuration updates documented
    - Example: Update deployment docs for new config keys.

### 📝 **PR Metadata**

- [ ] PR title is clear and descriptive
    - Example: "Add FHIR validation for patient reference"
- [ ] Description explains what and why
    - Example: "This PR adds validation to ensure patient references comply with FHIR."
- [ ] JIRA ticket linked
    - Example: "Resolves JIRA-1234"
- [ ] Appropriate reviewers assigned
    - Example: Add domain expert and tech lead as reviewers.
- [ ] Labels/tags applied correctly
    - Example: Add `bugfix`, `healthcare`, `validation` labels.
- [ ] Target branch is correct
    - Example: PR targets `develop` branch, not `main`.

### ✅ **Final Validation**

- [ ] All reviewer comments addressed
    - Example: All comments marked as resolved in PR.
- [ ] Conflicts resolved properly
    - Example: Merge conflicts fixed and verified before merge.
- [ ] Final build passes all quality gates
    - Example: SonarQube, coverage, and pipeline checks all pass.
- [ ] Ready for deployment to target environment
    - Example: PR marked as ready for QA or production deployment.

## Conclusion

If the analysis reveals a mature development process, identify opportunities for further standardization and automation to enhance efficiency while maintaining quality standards.

## Final Output

After completing the review, provide:

1. A filled out checklist with proper marks (✓, ✗, N/A) for each item
2. A comprehensive summary with:
   - Overall code quality assessment
   - Key strengths observed
   - Priority areas for improvement
   - Specific, actionable recommendations with code examples
   - Suggestions for process improvements

Your review should be thorough, specific to the actual code being reviewed, and include concrete examples from the code.
{{/if}}

{{#if (eq analysis_focus "Create Pull Request")}}

## Instructions

[Use the Azure Devops MCP server to create a Pull Request]

Create a pull request using the following steps:

I need to create a new Pull Request in Azure DevOps using the Model Context Protocol (MCP) server. Please use the mcp_ado_repo_create_pull_request tool with the following details:

Repository Name: Mcg.Davinci.Path
Source Branch: feature/dvt-2970-use-FHIRPatientId-as-PatientId
Target Branch: main
PR Title: PR Review for DVT-2970 - feature-dvt-2970-use-FHIRPatientId-as-PatientId
Work Item ID: 2970

For the PR description, please extract and format the content from this file: mcg-prompts/analysis/DVT-2970-Generate PR Review.md

Format the description with proper markdown including headings, bullet points, and sections to make it readable and professional.

After creating the PR, please provide me with the PR URL and confirmation that it was successfully created.
{{/if}}

Please format the response clearly with sections and bullet points for easy reading.

**File Output:**

Create an md file naming **{{ ticket_key }}**-**{{ analysis_focus }}** in analysis folder under mcg-prompts folder.
