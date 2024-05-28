(async () => {

  var keepalive = true;

  ['SIGINT', 'SIGTERM', 'SIGQUIT']
    .forEach(signal => process.on(signal, () => {
      keepalive = false;
      process.exit();
      return;
    }));

  while (keepalive) {
    continue;
  }

})();