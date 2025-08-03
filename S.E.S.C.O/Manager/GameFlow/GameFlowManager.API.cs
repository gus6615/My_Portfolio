using System.Linq;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public partial class GameFlowManager
    {
        public float GetUpdateSpeed() => _updateSpeed;
        public float GetDeltaTime() => Time.deltaTime * _updateSpeed;
        
        public void AddUpdateHandler(UpdateCallback callback, UpdatePriority priority = UpdatePriority.Normal)
        {
            _updateHandlers.Add(new PrioritizedUpdateHandler(callback, priority));
            isDirtyFlagOfUpdateHandler = true;
        }

        public void RemoveUpdateHandler(UpdateCallback callback)
        {
            foreach (var handler in _updateHandlers.ToList())
            {
                if (handler.UpdateCallback.Equals(callback))
                {
                    _updateHandlers.Remove(handler);
                }
            }
        }
        
        public void AddFixedUpdateHandler(UpdateCallback callback, UpdatePriority priority = UpdatePriority.Normal)
        {
            _fixedUpdateHandlers.Add(new PrioritizedUpdateHandler(callback, priority));
            isDirtyFlagOfFixedUpdateHandler = true;
        }

        public void RemoveFixedUpdateHandler(UpdateCallback callback)
        {
            foreach (var handler in _fixedUpdateHandlers.ToList())
            {
                if (handler.UpdateCallback.Equals(callback))
                {
                    _fixedUpdateHandlers.Remove(handler);
                }
            }
        }

        public T PushState<T>(object data = null) where T : StateBase, new ()
        {
            T state = new T();
            state.Initialize(data);
            _stateQueue.Enqueue(state);
            return state;
        }

        public void Pause()
        {
            _isPause = true;
        }

        public void Resume()
        {
            _isPause = false;
        }
        
        public void SetUpdateSpeed(float speed)
        {
            _updateSpeed = speed;
            UpdateSpeedChanged?.Invoke(speed);
        }
    }
}