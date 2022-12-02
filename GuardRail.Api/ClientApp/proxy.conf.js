const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:43900';

const proxyConfig = [
  {
    context: [
      '/api/**'
    ],
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
];

module.exports = proxyConfig;


{
  "/api": {
    "target": "http://localhost:3000",
      "secure": false
  }
}
