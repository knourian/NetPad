export class System {
    /**
     * Opens a URL in system-configured default browser.
     * @param url The URL to open.
     */
    public static async openUrlInBrowser(url: string): Promise<void> {
        if (this.isRunningInElectron()) {
            /* eslint-disable @typescript-eslint/no-var-requires */
            await require("electron").shell.openExternal(url);
            /* eslint-enable @typescript-eslint/no-var-requires */
        } else
            window.open(url, "_blank");
    }

    public static getPlatform() {
        if (this.isRunningInElectron()) {
            try {
                return require("process").platform;
            } catch {
                return undefined;
            }
        } else {
            return undefined;
        }
    }

    public static downloadFile(fileName: string, mimeType = "text/plain", base64: string) {
        const downloadLink = document.createElement("A") as HTMLAnchorElement;
        try {
            downloadLink.download = fileName;
            downloadLink.href = `data:${mimeType};base64,${base64}`;
            downloadLink.target = '_blank';
            downloadLink.click();
        } finally {
            downloadLink.remove();
        }
    }

    public static downloadTextAsFile(fileName: string, mimeType = "text/plain", text: string) {
        const downloadLink = document.createElement("A") as HTMLAnchorElement;
        try {
            downloadLink.download = fileName;
            downloadLink.href = `data:${mimeType};charset=utf-8,${encodeURIComponent(text)}`;
            downloadLink.target = '_blank';
            downloadLink.click();
        } finally {
            downloadLink.remove();
        }
    }

    /**
     * Determines if app is running in Electron.
     */
    public static isRunningInElectron(): boolean {
        return navigator.userAgent.toLowerCase().indexOf(' electron/') > -1;
    }
}
