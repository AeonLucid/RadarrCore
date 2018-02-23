using Radarr.Common.Messaging;

namespace Radarr.Core.Datastore
{
    public class ModelEvent<TModel> : IEvent
    {
        public TModel Model { get; set; }
        public ModelAction Action { get; set; }

        public ModelEvent(TModel model, ModelAction action)
        {
            Model = model;
            Action = action;
        }
    }
}
