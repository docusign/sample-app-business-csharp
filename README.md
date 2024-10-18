# Business Sample Application: React

## Introduction

The Business Sample App provides ISV developers with a practical example of how to build a quality in-product integration. This is a Single Page Application (SPA) leveraging the Docusign C# eSignature SDK. The SPA is created using React.js and .Net Web API. You can find a live instance at [https://business.sampleapps.docusign.com/](https://business.sampleapps.docusign.com/).

The Business Sample App demonstrates the following:

1. **Authentication** with Docusign via [JWT Grant](https://developers.docusign.com/platform/auth/jwt/) or [ACG](https://developers.docusign.com/platform/auth/authcode/).
2. **Send an Employment Contract:**
   This use case presents a scenario wherein the ISV’s app enables an employee to send an employment contract to a new hire.
   Key elements:
   - Composite templates
   - Envelope-level Connect notifications
   - Electronic or digital signature options (SBS)
   - Embedded sending
   - Remote signing via email notification
3. **T&Cs Contract:**
   This use case presents a scenario in which a sales employee is sending a standard Terms and Conditions document along with a Contract document to an external buyer.
   Key elements:
   - Composite templates
   - Envelope-level Connect notifications
   - Prepopulated field data
   - Recipient Authentication with Access Code
4. **Update Customer Profile Self-service:**
   This use case presents a scenario in which an online customer portal provides a means for customers to update their profiles. The app requires password authentication. As an added level of security, the embedded signing session will require that the signer pass an IDV check.
   Key elements:
   - Composite templates
   - Envelope-level Connect notifications
   - Embedded signing with login data mapped to security elements
   - IDV or SMS/Phone authentication
5. **Custom Quote:**
   This use case presents a scenario in which a sales employee is sending a generated custom quote to a prospective buyer.
   Key elements:
   - Composite templates with anchor tabs
   - Envelope-level Connect notifications
   - Remote signing via SMS delivery

## Prerequisites

- Create a Docusign [Developer Account](https://go.docusign.com/o/sandbox/).
- Create an application on the [Apps and Keys](https://admindemo.docusign.com/authenticate?goTo=appsAndKeys) page.
- Add redirect URIs:
  - `{ PROTOCOL }://{ DOMAIN }`
  - `{ PROTOCOL }://{ DOMAIN }/admin`
  - `{ PROTOCOL }://{ DOMAIN }/api/consentcallback`
- Press "ADD SECRET KEY" and save the generated key (it will be used later in "Settings configuration" section to configure {SecretKey})
- Press "GENERATE RSA" and save the generated key pairs (it will be used later in "Settings configuration" section to configure "private.key")
- Installed and configured [Node.js](https://nodejs.org/en/download)
- Installed and configured [Docker](https://www.docker.com/)
- Installed and configured [.Net 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### Settings configuration

Create a copy of the file appsettings-example.json, save the copy as appsettings.json, and fill in the data:

- {IntegrationKey} - integration key of the application created in section "Create an application on the Apps and Keys page" above (GUID)
- {SecretKey} - the application secret code that was generated in "Prerequisites" section
- Save generate private RSA key (section "Prerequisites") to the file \sample-app-business-csharp\Docusign.MyBusiness\DocuSign.MyBusiness\private.key
- {RedirectBaseUrl} - internal redirection URL that is used during the obtaining consent and embedded signing process
- {EventNotificationBaseUrl} - public base URL of the application that is used by Docusign Events webhook
- "TestAccountConnectionSettings" section:
  - {AuthBasePath} - base authentication URL that is used during loging with a test account
  - {BaseUri} - base API url that is used to call the API endpoints in case using the test account
  - {UserId} - ID of the test user
  - {AccontId} - ID of the account that is connected to the test user
- "CustomerProfile" section:
  - {Login} - hardcoded login that is used as a part of Use Case 3
  - {Password} - hardcoded password that is used as a part of Use Case 3:

## Local installation instructions (without Docker)

1. Clone the git repository to your local machine
1. Make the Settings configuration described above
1. Open a terminal and navigate to \sample-app-business-csharp\DocuSign.MyBusiness\DocuSign.MyBusiness\ClientApp folder
1. Install required client application packages running the following command in the terminal:
   ```
   npm install
   ```
1. Start the client application running the following command in the terminal:
   ```
   npm start
   ```
1. Open a new terminal and navigate to \sample-app-business-csharp\DocuSign.MyBusiness
1. Build the .Net solution:
   ```
   dotnet build --configuration Debug
   ```
1. Start the .NET application:
   ```
   dotnet run --project .\DocuSign.MyBusiness\DocuSign.MyBusiness.csproj --configuration Debug
   ```
1. Open a browser to [localhost:5000](http://localhost:5000) (if the page is not openned automatically).

## Local installation instructions (using Docker)

1. Clone the git repository to your local machine
1. Make the Settings configuration described above
1. Open a terminal in \sample-app-business-csharp directory.
1. Build the docker image running the following command in the terminal:
   ```
   docker build -f DocuSign.MyBusiness/DocuSign.MyBusiness/Dockerfile -t docusign-business .
   ```
1. Start the application (run the docker container) with the following command in the terminal:

   ```
   docker run -p 80:80 -d docusign-business
   ```

1. Open a browser to [localhost](http://localhost)

## License information

This repository uses the MIT License. See the [LICENSE](./LICENSE) file for more information.
