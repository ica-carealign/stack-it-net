StackIt.Net
===========

A web application for managing virtual machines in the cloud

Getting Started
---------------

After checking out the repository,

1. Install the [AWS SDK for .Net](http://aws.amazon.com/sdk-for-net/)
    * The configured profile should be named "default"
2. Follow the instructions in <repo>/src/WebPortal/PrivateSettings.config.template
3. ???
4. Profit

Authentication
--------------

There are currently two authentication modes.

1. Development - no external dependencies
    * Login with *user*/*user-password* i.e. john.smith/john.smith-password.
2. Crowd - Login with Crowd credentials.
    * Defaults to https://crowd.example.com:8443/crowd/services/SecurityServer

To change authentication methods, open the web.config and change /appSettings/add[@key="AuthenticationProvider"]/@value to "Crowd".
