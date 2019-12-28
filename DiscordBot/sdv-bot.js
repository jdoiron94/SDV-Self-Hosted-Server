const Discord = require('discord.js');
const logger = require('winston');
const fs = require('fs');
const config = require('./config.json');

// Configure logger settings
logger.remove(logger.transports.Console);
logger.add(new logger.transports.Console, {
    colorize: true
});
logger.level = 'debug';

// Initialize Discord Bot
var client = new Discord.Client();

client.on('ready', () => {
    logger.info('Connected');
});

client.on('message', (msg) => {
    if (msg.channel.id === config.channelId) {
      if (msg.content === '!code') {
        fs.readFile(config.inviteCodePath, 'utf-8', (err, data) => {
          if (err) {
            throw err;
          }
          msg.reply(`Invite code: ${data}`);
        });
      }
    }
});

client.login(config.token);