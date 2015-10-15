﻿using System;
using System.Linq;
using System.Linq.Expressions;
using NServiceBus.Saga;

namespace NServiceBus.SqlPersistence
{
    public class MessagePropertyMapper<TSagaData> where TSagaData : XmlSagaData, new()
    {
        SagaPropertyMapper<TSagaData> sagaPropertyMapper;
        Expression<Func<object>> correlationExpression;

        internal MessagePropertyMapper(SagaPropertyMapper<TSagaData> sagaPropertyMapper)
        {
            this.sagaPropertyMapper = sagaPropertyMapper;

            var sagaDataType = typeof(TSagaData);
            var correlationMember = sagaDataType
                .GetMembers()
                .FirstOrDefault(x => x.ContainsAttribute<CorrelationIdAttribute>());
            if (correlationMember == null)
            {
                return;
            }
            correlationExpression = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Property(
                        Expression.Constant(this, sagaDataType), correlationMember.Name), typeof(object)));
        }

        /// <summary>
        /// Specify how to map between <typeparamref name="TSagaData"/> and <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The message type to map to.</typeparam>
        /// <param name="messageProperty">An <see cref="Expression"/> that represents the message.</param>
        /// <returns>A <see cref="ToSagaExpression{TSagaData,TMessage}"/> that provides the fluent chained <see cref="ToSagaExpression{TSagaData,TMessage}.ToSaga"/> to link <paramref name="messageProperty"/> with <typeparamref name="TSagaData"/>.</returns>
        public void MapMessage<TMessage>(Expression<Func<TMessage, object>> messageProperty)
        {
            if (correlationExpression == null)
            {
                throw new Exception("No correlation member has been defined.");
            }
            sagaPropertyMapper.ConfigureMapping(messageProperty)
                .ToSaga(data => correlationExpression);
        }
    }
}