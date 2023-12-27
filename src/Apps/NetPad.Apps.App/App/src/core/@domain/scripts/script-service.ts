import {DI, IHttpClient} from "aurelia";
import {IEventBus, IScriptsApiClient, ScriptsApiClient} from "@domain";

export interface IScriptService extends IScriptsApiClient {}

export const IScriptService = DI.createInterface<IScriptService>();

export class ScriptService extends ScriptsApiClient implements IScriptService {

    constructor(@IEventBus private readonly eventBus: IEventBus, baseUrl?: string, @IHttpClient http?: IHttpClient) {
        super(baseUrl, http);
    }

    public override async updateCode(id: string, code: string, signal?: AbortSignal | undefined): Promise<void> {
        this.eventBus.publish("script-code-update-request", "started");

        try {
            return await super.updateCode(id, code, signal);
        } finally {
            this.eventBus.publish("script-code-update-request", "finished");
        }
    }
}
