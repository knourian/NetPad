import {Constructable, EventAggregator, IDisposable, ILogger} from "aurelia";
import {IEventBus, IIpcGateway} from "@domain";

export class EventBus extends EventAggregator implements IEventBus {
    private readonly logger: ILogger;

    constructor(@IIpcGateway readonly ipcGateway: IIpcGateway, @ILogger logger: ILogger) {
        super();
        this.logger = logger.scopeTo(nameof(EventBus));
    }

    public subscribeToServer<TMessage extends Constructable>(
        channel: string | Constructable,
        callback: (message: InstanceType<TMessage>, channel: string) => void): IDisposable {
        const channelName = typeof channel === 'string' ? channel : channel.name;

        const proxyCallback = (message: InstanceType<TMessage>, channel: string) => {
            callback(message, channel);
        };

        return this.ipcGateway.subscribe(channelName, proxyCallback);
    }
}

