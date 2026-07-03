"""One-shot patch: enable Run Everything in Cursor applicationUser storage."""
import json
import sqlite3
from pathlib import Path

DB = Path.home() / "AppData/Roaming/Cursor/User/globalStorage/state.vscdb"
KEY = "src.vs.platform.reactivestorage.browser.reactiveStorageServiceImpl.persistentStorage.applicationUser"
KEY2 = "src.vs.platform.reactivestorage.browser.reactiveStorageService.applicationUserPersistentStorage"


def patch(data: dict) -> dict:
    cs = data.setdefault("composerState", {})
    cs.update(
        {
            "autoAcceptWebSearchTool": True,
            "enableSmartAuto": False,
            "smartAutoRun": False,
            "autoRun": True,
            "fullAutoRun": True,
            "yoloEnableRunEverything": True,
            "yoloOutsideWorkspaceDisabled": False,
            "autoApplyFilesOutsideContext": True,
            "doNotShowFullYoloModeWarningAgain": True,
            "doNotShowYoloModeWarningAgain": True,
            "playwrightProtection": False,
        }
    )
    for mode in cs.get("modes4", []):
        mode["autoRun"] = True
        mode["fullAutoRun"] = True
        mode["smartModeAutoRun"] = False
        mode["shouldAutoApplyIfNoEditTool"] = True
    data["composerState"] = cs
    return data


def main() -> None:
    conn = sqlite3.connect(DB)
    cur = conn.cursor()
    for key in (KEY, KEY2):
        cur.execute("SELECT value FROM ItemTable WHERE key=?", (key,))
        row = cur.fetchone()
        if not row:
            continue
        cur.execute(
            "INSERT OR REPLACE INTO ItemTable (key, value) VALUES (?, ?)",
            (key, json.dumps(patch(json.loads(row[0])))),
        )
    conn.commit()
    cur.execute("SELECT value FROM ItemTable WHERE key=?", (KEY,))
    cs = json.loads(cur.fetchone()[0])["composerState"]
    print("patched OK")
    print("yoloEnableRunEverything:", cs.get("yoloEnableRunEverything"))
    print("smartModeAutoRun (agent):", next((m.get("smartModeAutoRun") for m in cs.get("modes4", []) if m.get("id") == "agent"), None))
    conn.close()


if __name__ == "__main__":
    main()
