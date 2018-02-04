# Sitefinity ReinventionBot
This bot is going to help us by providing each user the most relevant information. The verbosity of the bot will be configured by each user trough conversations with the bot itself. All needed information for integration should be provided by the user.

## Use Visual Studio 2017 for development 

### Build and debug
1. download source code zip and extract source in local folder
2. open {PROJ_NAME}.sln in Visual Studio
3. build and run the bot
4. download and run [botframework-emulator](https://emulator.botframework.com/)
5. connect the emulator to http://localhost:3987

### Publish back

In Visual Studio, right click on {PROJ_NAME} and select 'Publish'

For first time publish after downloading source code
1. In the publish profiles tab, click 'Import'
2. Browse to 'PostDeployScripts' and pick '{SITE_NAME}.publishSettings'



