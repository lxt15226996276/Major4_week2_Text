/** 本地服务器 vs 静态永久站点 的链接解析 */

function derivePublicBase() {
  const path = location.pathname.replace(/\/[^/]+\.html$/i, "").replace(/\/$/, "");
  return location.origin + path;
}

async function fetchSiteMeta() {
  try {
    const res = await fetch("site.json", { cache: "no-cache" });
    if (res.ok) return await res.json();
  } catch {
    /* static bundle may be opened via file:// */
  }
  return null;
}

async function getShareUrl() {
  const site = await fetchSiteMeta();
  if (site?.mode === "static") {
    return (site.publicUrl || derivePublicBase()).replace(/\/$/, "");
  }
  try {
    const res = await fetch("/api/info");
    const info = await res.json();
    const url = info.publicUrl || info.urls?.find((u) => !u.includes("127.0.0.1"));
    if (url) return url.replace(/\/$/, "");
  } catch {
    /* local server not running */
  }
  return null;
}

async function isStaticSite() {
  const site = await fetchSiteMeta();
  return site?.mode === "static";
}
