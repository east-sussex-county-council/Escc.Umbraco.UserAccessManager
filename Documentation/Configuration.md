# Configuring the Umbraco User Access Manager 

## Permissions

Permissions to access the tools in this application are controlled using two roles:

**WebServices**: Has full permission to the entire application.

**ServiceDesk**: Has permission to lookup a user and initiate a password reset.

These roles are registered in `web.config`, where the values are Active Directory group names:

	<appSettings>
	    <add key="SystemRole.WebServices" value="groupname" />
	    <add key="SystemRole.ServiceDesk" value="groupname" />
	</appSettings>

## Email

The following configuration options can be set in `web.config` to change the text of emails sent by this application:

	<appSettings>
		<!-- Duplicate setting used in different contexts -->
	    <add key="UmbracoBackOfficeUrl" value="https://example.org/umbraco" />
	    <add key="SiteUri" value="https://example.org/umbraco" />

	    <add key="WebAuthorsGuidanceUrl" value="https://example.org/help-pages-for-web-authors" />
	    <add key="WebAuthorsYammerUrl" value="https://www.yammer.com/organisation-name/group-for-web-authors" />
	</appSettings>

You can also control when emails are sent and where to:

	<appSettings>
		<!-- Duplicate setting used in different contexts -->
	    <add key="EmailTo" value="website-admin@example.org" />
	    <add key="WebStaffEmail" value="website-admin@example.org" />

		<!-- When to notify web authors and the admin team about pages due to expire, with default values shown -->
		<add key="NoOfDaysFrom" value="14" />
		<add key="EmailWebStaffAtDays" value="3" />

		<!-- Override to avoid sending emails to web authors during development -->
		<add key="ForceSendTo" value="developer-override@example.org" />
	</appSettings>

## Umbraco user type

The Umbraco user type which is managed by this tool is identified by the user type alias in the `web.config` of the Umbraco instance where `Escc.Umbraco.UserAccessWebService` is installed.

The "NewUser" User Type should be set up with default permissions of "Browse" only. If this is not set, the permissions tree fails to load with an `IndexOutOfRangeException`.

The permission set assigned by the permissions tree is also defined in `web.config`.

	<appSettings>
    	<add key="WebAuthorUserType" value="NewUser" />
		<add key="defaultUserPermissions" value="ACFKUP" />
	</appSettings>

