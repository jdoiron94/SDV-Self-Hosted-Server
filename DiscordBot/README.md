# Self Hosted Server Discord Bot
Provided with this mod is an optional Discord bot that will respond to the command `!code` in a specific channel you have provided and will respond back with the invitation code to join your Stardew Valley server.

For the purposes of this document, it is assumed you already have a local NodeJS instance. If you need to install NodeJS, please refer to the LTS version located [here](https://nodejs.org/en/).

## Installation
You will need to start by CDing into the directory you have extracted the `DiscordBot` directory to.

    cd "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\SelfHostedServer\DiscordBot"

You will then need to use npm to install the required dependencies.

    $ npm install

Once you have installed the required dependencies, you will need to edit the `config.json` file to add your bot's configuration settings.

Here are the following properties in the configuration file and descriptions of the values expected for each key.

* `token` (String): token The client secret used to authenticate the Discord bot
* `channelId` (String): The channel id the bot should monitor for message triggers. This is not the name of the channel
* `inviteCodePath` (String): The fully qualified path to the invite code file the mod saves. This will be the path to the SelfHostedMod directory concatenated with the invite code file name. Example: `C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\SelfHostedServer\InviteCode.txt`