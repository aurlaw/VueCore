export default class WebWorker {
    constructor(worker) {
      const code = worker.toString();
      // eslint-disable-next-line no-undef
      const blob = new Blob(["(" + code + ")()"]);
      // eslint-disable-next-line no-undef
      return new Worker(URL.createObjectURL(blob));
    }
  }