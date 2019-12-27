using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.IO;

namespace SelfHostedServer
{
    public class ModEntry : Mod
    {

        private int clients;
        private int bedX;
        private int bedY;
        private int titleMenuTicks = 30;

        private bool asleep;

        private string inviteCode;

        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.Rendered += OnRendered;
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Game1.activeClickableMenu is ShippingMenu)
            {
                Monitor.Log($"SavingEvent event: [Game1.activeClickableMenu] => {Game1.activeClickableMenu}", LogLevel.Debug);
                Helper.Reflection.GetMethod(Game1.activeClickableMenu, "okClicked").Invoke();
            }
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                int players = Game1.otherFarmers.Count;
                string newInviteCode = Game1.server.getInviteCode();
                
                if (newInviteCode != inviteCode)
                {
                    inviteCode = newInviteCode;
                    string inviteFilePath = Path.Combine(Constants.ExecutionPath, "Mods", "SelfHostedServer", "InviteCode.txt");
                    Monitor.Log($"OneSecondUpdateTicked event: [inviteCode] => {inviteCode}", LogLevel.Debug);

                    using (StreamWriter writer = new StreamWriter(inviteFilePath))
                    {
                        writer.WriteLine(newInviteCode);
                    }
                }
                
                if (clients != players)
                {
                    clients = players;
                    Monitor.Log($"OneSecondUpdateTicked event: [clients] => {clients}", LogLevel.Debug);
                    if (players >= 1)
                    {
                        Game1.paused = false;
                    }
                    else
                    {
                        Game1.paused = true;
                    }
                }
                else if (players == 0 && !Game1.paused)
                {
                    Monitor.Log($"OneSecondUpdateTicked event: [clients] => {clients}", LogLevel.Debug);
                    Game1.paused = true;
                }
            }
            else
            {
                if (Game1.activeClickableMenu is TitleMenu)
                {
                    if (titleMenuTicks <= 0)
                    {
                        Monitor.Log($"OneSecondUpdateTicked event: [Game1.activeClickableMenu] => {Game1.activeClickableMenu}", LogLevel.Debug);
                        if (!Directory.Exists(Path.Combine(Constants.SavesPath, config.SaveData.FileName)))
                        {
                            Monitor.Log($"OneSecondUpdateTicked event: Save file does not exist ({config.SaveData.FileName})", LogLevel.Warn);
                        }
                        else
                        {
                            if (config.SaveData.Multiplayer)
                            {
                                Game1.multiplayerMode = 2;
                            }
                            SaveGame.Load(config.SaveData.FileName);
                            titleMenuTicks = 30;
                            Monitor.Log($"OneSecondUpdateTicked event: Loaded save file => {config.SaveData.FileName}", LogLevel.Debug);
                            if (Game1.activeClickableMenu is TitleMenu)
                            {
                                Game1.activeClickableMenu.exitThisMenu(false);
                            }
                        }
                    }
                    else
                    {
                        titleMenuTicks -= 1;
                    }
                }
            }
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Context.IsWorldReady && !Game1.paused && !asleep && e.NewTime >= 700)
            {
                Monitor.Log($"TimeChanged event: [TimeChangedEventArgs.NewTime] => {e.NewTime}", LogLevel.Debug);
                Sleep();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            asleep = false;
            Monitor.Log($"DayStarted event: [asleep] => {asleep}", LogLevel.Debug);
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (inviteCode != null)
            {
                DrawText(5, 340, Game1.dialogueFont, $"Player count: {clients}");
                DrawText(5, 420, Game1.dialogueFont, $"Invite code: {inviteCode}");
            }
        }

        private void DrawText(int x, int y, SpriteFont font, string message)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;
            int width = (int) font.MeasureString(message).X + 32;
            int height = (int) font.MeasureString(message).Y + 21;

            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            Utility.drawTextWithShadow(spriteBatch, message, font, new Vector2(x + 16, y + 16), Game1.textColor);
        }

        private void Sleep()
        {
            asleep = true;
            int houseUpgradeLevel = Game1.player.HouseUpgradeLevel;
            if (houseUpgradeLevel == 0)
            {
                bedX = 9;
                bedY = 9;
            }
            else if (houseUpgradeLevel == 1)
            {
                bedX = 21;
                bedY = 4;
            }
            else
            {
                bedX = 27;
                bedY = 13;
            }
            Game1.warpFarmer("Farmhouse", bedX, bedY, false);
            Helper.Reflection.GetMethod(Game1.currentLocation, "startSleep").Invoke();
        }
    }
}