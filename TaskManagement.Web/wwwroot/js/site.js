(function () {
  const toastContainer = document.getElementById("toastContainer");
  const notificationList = document.getElementById("notificationList");
  const notificationBadge = document.getElementById("notificationBadge");
  let notificationCount = 0;

  function mapCategoryToBootstrap(category) {
    switch ((category || "").toLowerCase()) {
      case "success":
        return "text-bg-success";
      case "warning":
        return "text-bg-warning";
      case "danger":
        return "text-bg-danger";
      case "info":
      default:
        return "text-bg-info";
    }
  }

  function showToast(message, category) {
    if (!toastContainer) return;
    const id = "toast-" + Date.now();
    const headerClass = mapCategoryToBootstrap(category);
    const el = document.createElement("div");
    el.className = "toast";
    el.id = id;
    el.setAttribute("role", "alert");
    el.setAttribute("aria-live", "assertive");
    el.setAttribute("aria-atomic", "true");
    el.innerHTML =
      '<div class="toast-header ' +
      headerClass +
      '">' +
      '<strong class="me-auto">Notification</strong>' +
      '<button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>' +
      "</div>" +
      '<div class="toast-body">' +
      escapeHtml(message) +
      "</div>";
    toastContainer.appendChild(el);
    const toast = new bootstrap.Toast(el, { delay: 6000 });
    el.addEventListener("hidden.bs.toast", function () {
      el.remove();
    });
    toast.show();
  }

  function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
  }

  function addToList(message, category) {
    if (!notificationList) return;
    const li = document.createElement("li");
    li.className = "px-3 py-2 border-bottom small";
    li.innerHTML =
      '<span class="badge ' +
      mapCategoryToBootstrap(category) +
      ' me-2">' +
      escapeHtml(category || "info") +
      "</span>" +
      escapeHtml(message);
    const header = notificationList.firstElementChild;
    if (header) {
      header.after(li);
    } else {
      notificationList.appendChild(li);
    }
    notificationCount += 1;
    if (notificationBadge) {
      notificationBadge.textContent = String(notificationCount);
      notificationBadge.classList.remove("d-none");
    }
  }

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveNotification", function (message, category) {
    showToast(message, category);
    addToList(message, category);
  });

  connection
    .start()
    .catch(function (err) {
      console.warn("SignalR connection failed:", err);
    });
})();
