using System.Collections.Generic;

namespace ToolsLibrary.EquipPart_Logic
{
    public class FsmManager
    {
        private EquipSkillBase currentState;

        public Dictionary<string, EquipSkillBase> rocketStates;

        public FsmManager()
        {
            if (rocketStates == null)
                rocketStates = new Dictionary<string, EquipSkillBase>();
            else
                rocketStates.Clear();
        }

        public void AddState(string rs, EquipSkillBase ifs)
        {
            if (!rocketStates.ContainsKey(rs))
            {
                rocketStates.Add(rs, ifs);
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public bool TransitionState(string nextState)
        {
            if (rocketStates == null) return false;
            if (string.IsNullOrEmpty(nextState)) return true;
            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = rocketStates[nextState];

            currentState.OnEnter(null);
            return true;
        }

        public void Update()
        {
            currentState?.OnUpdate();
        }

        public void EndState()
        {
            currentState?.OnExit();
            rocketStates?.Clear();
            rocketStates = null;
        }
    }
}