namespace picomessenger.tests;

[TestFixture]
public class RegisterDelegates
{
    public class RegisterDelegatesTestingTarget
    {
        private readonly IDisposable clearRegistration;

        public RegisterDelegatesTestingTarget(IMessageSubscriberRegistry subscriberRegistry)
        {
            this.clearRegistration = subscriberRegistry
                .Register<SomeMessage>(this.SomeMessageTarget)
                .And<SomeMessage>(this.SomeMessageTargetAsync);
        }

        public virtual void SomeMessageTarget(SomeMessage m)
        {
        }

        public virtual Task SomeMessageTargetAsync(SomeMessage m)
        {
            return Task.CompletedTask;
        }

        public void ReleaseRegistration()
        {
            this.clearRegistration.Dispose();
        }
    }

    [Test]
    public async Task DoRegister()
    {
        var messenger = new PicoMessenger();

        var target = Substitute.ForPartsOf<RegisterDelegatesTestingTarget>(messenger);

        await messenger.PublishMessageAsync(new SomeMessage());
        
        target.Received(1).SomeMessageTarget(Arg.Any<SomeMessage>());
        await target.Received(1).SomeMessageTargetAsync(Arg.Any<SomeMessage>());

        target.ReleaseRegistration();
        target.ClearReceivedCalls();
        
        await messenger.PublishMessageAsync(new SomeMessage());
        
        target.DidNotReceive().SomeMessageTarget(Arg.Any<SomeMessage>());
        await target.DidNotReceive().SomeMessageTargetAsync(Arg.Any<SomeMessage>());
    }

    public record SomeMessage;
}