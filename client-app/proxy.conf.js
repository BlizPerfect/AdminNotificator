const proxyAddress = 'http://localhost:44406';

module.exports = {
  '/api': {
    target: proxyAddress,
    secure: false,
    changeOrigin: true,
  },
};
