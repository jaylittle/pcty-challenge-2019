/* API Client Library */
let self = module.exports = {
  fixUrl(currentUrl) {
    if (currentUrl.startsWith("/"))
    {
      let baseUrl = document.getElementsByTagName('base')[0].href;
      baseUrl = baseUrl.endsWith("/") ? baseUrl.substring(0, baseUrl.length - 1) : baseUrl;
      return `${baseUrl}${currentUrl}`;
    }
    return currentUrl;
  },
  state: {
    isAuthorized(tenantId) {
      return Vue.http.get(self.fixUrl(`/api/state/isAuthorized`));
    }
  },
  token: {
    refresh() {
      return Vue.http.get(self.fixUrl('/token/refresh'));
    }
  }
};