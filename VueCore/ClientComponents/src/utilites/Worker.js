
// import users from '../data/users';
// const users = require('../data/users');

export default () => {
  // eslint-disable-next-line no-undef
    self.addEventListener("message", e => {
      // eslint-disable-line no-restricted-globals
      if (!e) return;
      var userData = e.data.users;
      if(!userData) return;
      var f = Function('"use strict";return (' + userData + ')');// convert user function string to function which returns a closure
      const users = f()();// execute
      // eslint-disable-next-line no-undef
      postMessage(users);

    });
  };