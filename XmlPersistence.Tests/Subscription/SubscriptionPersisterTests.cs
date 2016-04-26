using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Unicast.Subscriptions;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
#if (!DEBUG)
[Explicit]
#endif
public class SubscriptionPersisterTests
{

    static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=SqlPersistenceTests;Integrated Security=True";
    static string endpointName = "Endpoint";
    SubscriptionPersister persister;

    [SetUp]
    public void SetUp()
    {
        SetUpAsync().Await();
    }

    async Task SetUpAsync()
    {
        await DbBuilder.ReCreate(connectionString, endpointName);
        persister = new SubscriptionPersister(connectionString, "dbo", endpointName);
    }

    [Test]
    public void Subscribe()
    {
        var type1 = new MessageType("type1", new Version(0, 0, 0, 0));
        var type2 = new MessageType("type2", new Version(0, 0, 0, 0));
        var messageTypes = new List<MessageType>
        {
            type1,
            type2,
        };
        persister.Subscribe("address1@machine1".ToSubscriber(), type1, null).Await();
        persister.Subscribe("address1@machine1".ToSubscriber(), type2, null).Await();
        persister.Subscribe("address2@machine2".ToSubscriber(), type1, null).Await();
        persister.Subscribe("address2@machine2".ToSubscriber(), type2, null).Await();
        var result = persister.GetSubscriberAddressesForMessage(messageTypes, null).Result.Select(x => x.ToAddress());
        ObjectApprover.VerifyWithJson(result);
    }

    [Test]
    public void Subscribe_duplicate_add()
    {
        var type1 = new MessageType("type1", new Version(0, 0, 0, 0));
        var type2 = new MessageType("type2", new Version(0, 0, 0, 0));
        var messageTypes = new List<MessageType>
        {
            type1,
            type2,
        };
        persister.Subscribe("address1@machine1".ToSubscriber(), type1, null).Await();
        persister.Subscribe("address1@machine1".ToSubscriber(), type2, null).Await();
        persister.Subscribe("address1@machine1".ToSubscriber(), type1, null).Await();
        persister.Subscribe("address1@machine1".ToSubscriber(), type2, null).Await();
        var result = persister.GetSubscriberAddressesForMessage(messageTypes, null).Result.Select(x => x.ToAddress());
        ObjectApprover.VerifyWithJson(result);
    }

    [Test]
    public void Unsubscribe()
    {
        var message2 = new MessageType("type2", new Version(0, 0));
        var message1 = new MessageType("type1", new Version(0, 0));
        var messageTypes = new List<MessageType>
        {
            message2,
            message1,
        };
        var address1 = "address1@machine1".ToSubscriber();
        persister.Subscribe(address1, message2, null).Await();
        persister.Subscribe(address1, message1, null).Await();
        var address2 = "address2@machine2".ToSubscriber();
        persister.Subscribe(address2, message2, null).Await();
        persister.Subscribe(address2, message1, null).Await();
        persister.Unsubscribe(address1, message2, null).Await();
        var result = persister.GetSubscriberAddressesForMessage(messageTypes, null).Result.Select(x => x.ToAddress());
        ObjectApprover.VerifyWithJson(result);
    }
}