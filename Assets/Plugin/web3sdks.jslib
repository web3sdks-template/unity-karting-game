var plugin = {
  Web3sdksInvoke: function (taskId, route, payload, cb) {
    // convert taskId from pointer to str and allocate it to keep in memory
    var id = UTF8ToString(taskId);
    var idSize = lengthBytesUTF8(id) + 1;
    var idPtr = _malloc(idSize);
    stringToUTF8(id, idPtr, idSize);
    // execute bridge call
    window.bridge
      .invoke(UTF8ToString(route), UTF8ToString(payload))
      .then((returnStr) => {
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        // callback into unity
        dynCall_viii(cb, idPtr, buffer, null);
      })
      .catch((err) => {
        console.error("Web3sdksSDK invoke error", err);
        var msg = err.reason || err.message;
        var bufferSize = lengthBytesUTF8(msg) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(msg, buffer, bufferSize);
        dynCall_viii(cb, idPtr, null, buffer);
      });
  },
  Web3sdksInitialize: function (chain, options) {
    window.bridge.initialize(UTF8ToString(chain), UTF8ToString(options));
  },
  Web3sdksConnect: function (taskId, wallet, chainId, cb) {
    // convert taskId from pointer to str and allocate it to keep in memory
    var id = UTF8ToString(taskId);
    var idSize = lengthBytesUTF8(id) + 1;
    var idPtr = _malloc(idSize);
    stringToUTF8(id, idPtr, idSize);
    // execute bridge call
    window.bridge
      .connect(UTF8ToString(wallet), chainId)
      .then((address) => {
        if (address) {
          var bufferSize = lengthBytesUTF8(address) + 1;
          var buffer = _malloc(bufferSize);
          stringToUTF8(address, buffer, bufferSize);
          dynCall_viii(cb, idPtr, buffer, null);
        } else {
          dynCall_viii(cb, idPtr, null, null);
        }
      })
      .catch((err) => {
        var msg = err.message;
        var bufferSize = lengthBytesUTF8(msg) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(msg, buffer, bufferSize);
        dynCall_viii(cb, idPtr, null, buffer);
      });
  },
  Web3sdksSwitchNetwork: async function (taskId, chainId, cb) {
    // convert taskId from pointer to str and allocate it to keep in memory
    var id = UTF8ToString(taskId);
    var idSize = lengthBytesUTF8(id) + 1;
    var idPtr = _malloc(idSize);
    stringToUTF8(id, idPtr, idSize);
    // execute bridge call
    window.bridge
      .switchNetwork(chainId)
      .then(() => {
        dynCall_viii(cb, idPtr, null, null);
      })
      .catch((err) => {
        var msg = err.message;
        var bufferSize = lengthBytesUTF8(msg) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(msg, buffer, bufferSize);
        dynCall_viii(cb, idPtr, null, buffer);
      });
  },
  Web3sdksDisconnect: async function (taskId, cb) {
    // convert taskId from pointer to str and allocate it to keep in memory
    var id = UTF8ToString(taskId);
    var idSize = lengthBytesUTF8(id) + 1;
    var idPtr = _malloc(idSize);
    stringToUTF8(id, idPtr, idSize);
    // execute bridge call
    window.bridge
      .disconnect()
      .then(() => {
        dynCall_viii(cb, idPtr, idPtr, null);
      })
      .catch((err) => {
        var msg = err.message;
        var bufferSize = lengthBytesUTF8(msg) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(msg, buffer, bufferSize);
        dynCall_viii(cb, idPtr, null, buffer);
      });
  },
  Web3sdksFundWallet: async function (taskId, payload, cb) {
    // convert taskId from pointer to str and allocate it to keep in memory
    var id = UTF8ToString(taskId);
    var idSize = lengthBytesUTF8(id) + 1;
    var idPtr = _malloc(idSize);
    stringToUTF8(id, idPtr, idSize);
    // execute bridge call
    window.bridge
      .fundWallet(UTF8ToString(payload))
      .then(() => {
        dynCall_viii(cb, idPtr, idPtr, null);
      })
      .catch((err) => {
        var msg = err.message;
        var bufferSize = lengthBytesUTF8(msg) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(msg, buffer, bufferSize);
        dynCall_viii(cb, idPtr, null, buffer);
      });
  },
};

mergeInto(LibraryManager.library, plugin);
