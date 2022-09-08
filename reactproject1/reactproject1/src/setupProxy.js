const { createProxyMiddleware } = require('http-proxy-middleware');

const context = [
    "/api/test/workwithfiles",
    "/api/meters",
    "/api/Auth",
    "/api/indications/EnergyIndications/",
    "/api/indications/PowerIndications/",
    "/api/ReportXML80020/",
    "/api/shedules/",
    "/api/communicpoints/"
];

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context,{
        target: 'http://192.168.0.64:80',
        secure: false
    });

    app.use(appProxy);
};
