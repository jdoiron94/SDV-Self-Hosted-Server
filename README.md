# SDV-Self-Hosted-Server
A self-hosted server mod for use with Stardew Valley.

## Installation
You will need to start by CDing into the directory you have extracted the `SelfHostedServer` directory to.

    cd "C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\SelfHostedServer"

You will then need to rename the `config-dist.json` file to `config.json` and then add mod's configuration settings.

Here are the following properties in the configuration file and descriptions of the values expected for each key.

* `TimeToSleep` (integer): The time the bot should go to sleep, in military time. Note: Do not pad the hour with a leading 0. Example: `630` is a good value, `0630` is a bad value.
* `SaveData.FileName` (String): The file name of the save from the `Saves` directory to automatically load when the game is opened. Example: `Jacob_202268129`
* `SaveData.Multiplayer` (boolean): Whether or not the save instance is multiplayer