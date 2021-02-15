'use strict';

const queryItems = (el, nodes) => {
  const liItems = el.querySelectorAll(":scope > li");

  let i = 0;
  for (const liItem of liItems) {
    const targetElement = liItem.querySelector(":scope > a > .xaf-action");
    const captionItem = targetElement.querySelector(":scope > span");

    const node = {
      index: i,
      caption: captionItem.innerText,
      children: [],
      targetElement
    }
    i++;
    nodes.push(node);

    const nestedUl = liItem.querySelector(":scope > ul");
    if (nestedUl) {
      queryItems(nestedUl, node.children);
    }
  }
}

let injectedItems = [];
let virtualNavigationControl = [];

let disposables = [];

const clearBadges = () => {
  for (const el of [...injectedItems]) {
    injectedItems = injectedItems.filter(item => item !== el);
    el.remove();
  }
}

export function xenialAttachBadges(items) {
  const xenialStyles = document.getElementById("xenialBadgesStyles");

  if (!xenialStyles) {
    document.getElementsByTagName("head")[0].insertAdjacentHTML(
      "beforeend",
      `<link rel="stylesheet" href="_content/Xenial.Framework.Badges.Blazor/Xenial.Framework.Badges.Blazor.css" id="xenialBadgesStyles" />`);
  }

  items = items || [];

  const navMenu = document.querySelector(".xaf-navmenu");

  const queryNavigationControl = () => {
    virtualNavigationControl = [];
    var rootUl = navMenu.getElementsByTagName("ul")[0];
    queryItems(rootUl, virtualNavigationControl);
  }

  const applyBadge = (virtualNavItem) => {
    if (virtualNavItem.badge && virtualNavItem.badge.badge) {
      const badge = document.createElement("div");
      badge.innerText = virtualNavItem.badge.badge;
      badge.classList.add("xenial-notification-badge");
      if (virtualNavItem.badge.badgeType) {
        badge.classList.add(`xenial-notification-badge__${virtualNavItem.badge.badgeType.toLowerCase()}`);
      }
      injectedItems.push(badge);
      virtualNavItem.targetElement.insertAdjacentElement("beforeend", badge);
    }
  }

  const mergeTrees = (virtualNavItems, navItems) => {
    for (const virtualNavItem of virtualNavItems) {
      const canidates = navItems.filter(navItem => navItem.caption === virtualNavItem.caption && navItem.index === virtualNavItem.index);
      if (canidates && canidates.length > 0) {
        virtualNavItem.badge = {
          badge: canidates[0].badge,
          badgeType: canidates[0].badgeType,
        };
        mergeTrees(virtualNavItem.children, canidates[0].children);
      }
      else {
        virtualNavItem.badge = {
          badge: undefined,
          badgeType: undefined,
          children: []
        }
      }
    }
  }

  const applyBadgeItems = (virtualNavItems) => {
    for (const virtualNavItem of virtualNavItems) {
      applyBadge(virtualNavItem);
      applyBadgeItems(virtualNavItem.children);
    }
  }

  const applyBadges = () => {
    clearBadges();
    queryNavigationControl();
    mergeTrees(virtualNavigationControl, items);
    applyBadgeItems(virtualNavigationControl);
  }

  const delay = 50;

  applyBadges();

  setTimeout(() => {
    applyBadges();
  }, delay);

  if (navMenu instanceof HTMLElement) {

    const mouseUp = () => {
      queueMicrotask(() => {
        setTimeout(() => {
          applyBadges();
        }, delay);
      });
    };

    navMenu.addEventListener("mouseup", mouseUp);

    disposables.push(() => {
      navMenu.removeEventListener("mouseup", mouseUp);
    });
  }
}

export function xenialDeattachBadges() {

  clearBadges();

  for (const disposable of disposables) {
    disposable();
  }

  disposables = [];
}
