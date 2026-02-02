using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;
using UnityEditor.Animations;

namespace CRAssetEditorCore_v1_3
{
    public class CRAnimatorControllerUtility
    {
        #region 찾기
        // 레이어 번호 찾기.
        static public int FindLayerIndex(RuntimeAnimatorController source, string layerName)
        {
            return FindLayerIndex((AnimatorController)source, layerName);
        }

        static public int FindLayerIndex(AnimatorController source, string layerName)
        {
            for (int i = 0; i < source.layers.Length; i++)
            {
                if (source.layers[i].name.Equals(layerName))
                    return i;
            }

            return -1;
        }

        // 레이어 찾기.
        static public AnimatorControllerLayer FindLayer(RuntimeAnimatorController source, string layerName)
        {
            return FindLayer((AnimatorController)source, layerName);
        }

        static public AnimatorControllerLayer FindLayer(AnimatorController source, string layerName)
        {
            for (int i = 0; i < source.layers.Length; i++)
            {
                if (source.layers[i].name.Equals(layerName))
                    return source.layers[i];
            }

            return null;
        }

        // State 찾기.
        static public AnimatorState FindState(AnimatorStateMachine source, string stateName)
        {
            foreach (ChildAnimatorState childState in source.states)
            {
                if (childState.state.name.Equals(stateName))
                    return childState.state;
            }

            return null;
        }

        // State 찾기 (같은 이름)
        static public AnimatorState FindState(AnimatorStateMachine source, AnimatorState state)
        {
            if (!state)
                return null;

            foreach (ChildAnimatorState childState in source.states)
            {
                if (childState.state.name.Equals(state.name))
                    return childState.state;
            }

            return null;
        }

        // StateMachine 찾기.
        static public AnimatorStateMachine FindStateMachine(AnimatorStateMachine source, string stateName)
        {
            foreach (ChildAnimatorStateMachine childState in source.stateMachines)
            {
                if (childState.stateMachine.name.Equals(stateName))
                    return childState.stateMachine;
            }

            return null;
        }

        // StateMachine 찾기 (같은 이름)
        static public AnimatorStateMachine FindStateMachine(AnimatorStateMachine source, AnimatorStateMachine stateMachine)
        {
            if (!stateMachine)
                return null;

            foreach (ChildAnimatorStateMachine childState in source.stateMachines)
            {
                if (childState.stateMachine.name.Equals(stateMachine.name))
                    return childState.stateMachine;
            }

            return null;
        }

        // State 경로 찾기.
        static public string GetStatePath(AnimatorState source, AnimatorStateMachine root)
        {
            string path = "";

            foreach (ChildAnimatorState state in root.states)
            {
                if (state.state == source)
                    return state.state.name;
            }

            foreach (ChildAnimatorStateMachine stateMachine in root.stateMachines)
            {
                path = GetStatePath(source, stateMachine.stateMachine);
                if (path != "")
                {
                    path = stateMachine.stateMachine.name + "/" + path;
                }

                foreach (ChildAnimatorState state in stateMachine.stateMachine.states)
                {
                    if (state.state == source)
                    {
                        return stateMachine.stateMachine.name + "/" + state.state.name;
                    }
                }
            }

            return path;
        }

        // StateMachine 경로 찾기.
        static public string GetStateMachinePath(AnimatorStateMachine source, AnimatorStateMachine root)
        {
            string path = "";

            foreach (ChildAnimatorStateMachine stateMachine in root.stateMachines)
            {
                if (stateMachine.stateMachine == source)
                    return stateMachine.stateMachine.name;
            }

            foreach (ChildAnimatorStateMachine stateMachine in root.stateMachines)
            {
                path = GetStateMachinePath(source, stateMachine.stateMachine);
                if (path != "")
                {
                    path = stateMachine.stateMachine.name + "/" + path;
                }

                if (stateMachine.stateMachine == source)
                {
                    return stateMachine.stateMachine.name;
                }
            }

            return path;
        }

        // 경로로 State 찾기.
        static public AnimatorState FindStateToPath(string path, AnimatorStateMachine root)
        {
            string[] paths = path.Split('/');

            AnimatorStateMachine current = root;
            for (int i = 0; i < paths.Length - 1; i++)
            {
                current = FindStateMachine(current, paths[i]);
            }

            return FindState(current, paths[paths.Length - 1]);
        }

        // 경로로 StateMachine 찾기.
        static public AnimatorStateMachine FindSStateMachineToPath(string path, AnimatorStateMachine root)
        {
            string[] paths = path.Split('/');

            AnimatorStateMachine current = root;
            for (int i = 0; i < paths.Length - 1; i++)
            {
                current = FindStateMachine(current, paths[i]);
            }

            return FindStateMachine(current, paths[paths.Length - 1]);
        }

        // 이름으로 파라미터 찾기.
        public static AnimatorControllerParameter FindParameter(AnimatorController source, string name)
        {
            foreach (AnimatorControllerParameter parameter in source.parameters)
            {
                if (parameter.name.Equals(name))
                    return parameter;
            }

            return null;
        }
        #endregion

        #region 복사
        // 모든 요소 병합.
        public static void Merge(RuntimeAnimatorController  target, RuntimeAnimatorController source)
        {
            Merge((AnimatorController)target, (AnimatorController)source);
        }
        public static void Merge(AnimatorController target, AnimatorController source)
        {
            CopyAllParameter(target, source);
            CopyAllLayers(target, source);
        }

        // 모든 레이어 복사.
        public static void CopyAllLayers(RuntimeAnimatorController target, RuntimeAnimatorController source)
        {
            CopyAllLayers((AnimatorController)target, (AnimatorController)source);
        }

        static public void CopyAllLayers(AnimatorController target, AnimatorController source)
        {
            for (int i = 0; i < source.layers.Length; i++)
            {
                bool isDefault = (i == 0);
                CopyLayer(target, source.layers[i], isDefault);
            }
        }

        // 레이어 복사.
        public static void CopyLayer(RuntimeAnimatorController target, AnimatorControllerLayer layer, bool isDefault)
        {
            CopyLayer((AnimatorController)target, layer, isDefault);
        }

        static public void CopyLayer(AnimatorController target, AnimatorControllerLayer layer, bool isDefault)
        {
            // 기존 레이어 삭제.
            int oldLayerIndex = FindLayerIndex(target, layer.name);
            if (oldLayerIndex != -1)
                target.RemoveLayer(oldLayerIndex);

            // 새 레이어 생성.
            AnimatorControllerLayer newLayer = new AnimatorControllerLayer()
            {
                name = layer.name,
                avatarMask = layer.avatarMask,
                blendingMode = layer.blendingMode,
                defaultWeight = isDefault ? 1 : layer.defaultWeight,
            };

            // StateMechine 복사.
            AnimatorStateMachine newStateMachine = new AnimatorStateMachine();
            AssetDatabase.AddObjectToAsset(newStateMachine, target);

            CopyStateMachine(newStateMachine, layer.stateMachine);
            newLayer.stateMachine = newStateMachine;

            // 레이어 추가.
            target.AddLayer(newLayer);
        }

        // StateMachine 복사.
        public static void CopyStateMachine(AnimatorStateMachine target, AnimatorStateMachine source)
        {
            target.name = source.name;
            target.hideFlags = source.hideFlags;

            // State 복사.
            foreach (ChildAnimatorState childState in source.states)
            {
                AnimatorState newState = CopyStateObject(childState.state);
                AssetDatabase.AddObjectToAsset(newState, target);

                // Motion 추가
                if (childState.state.motion)
                {
                    newState.motion = (childState.state.motion.GetType() == typeof(BlendTree)) ?
                        CopyBlendTree(newState, (BlendTree)childState.state.motion) :
                        childState.state.motion;
                }

                // Behaviour 복사
                StateMachineBehaviour[] newBehaviour = new StateMachineBehaviour[childState.state.behaviours.Length];
                for (int i = 0; i < childState.state.behaviours.Length; i++)
                {
                    newBehaviour[i] = CopyBehaviourObject(childState.state.behaviours[i]);
                    AssetDatabase.AddObjectToAsset(newBehaviour[i], target);
                }
                newState.behaviours = newBehaviour;

                target.AddState(newState, childState.position);
            }

            // SubStateMachine 복사.
            foreach (ChildAnimatorStateMachine stateMachines in source.stateMachines)
            {
                AnimatorStateMachine newStateMachine = new AnimatorStateMachine();
                AssetDatabase.AddObjectToAsset(newStateMachine, target);

                target.AddStateMachine(newStateMachine, stateMachines.position);
                CopyStateMachine(newStateMachine, stateMachines.stateMachine);
            }

            // Base State 위치 설정.
            target.entryPosition = source.entryPosition;
            target.exitPosition = source.exitPosition;
            target.anyStatePosition = source.anyStatePosition;
            target.parentStateMachinePosition = source.parentStateMachinePosition;

            // 시작 State.
            target.defaultState = FindState(target, source.defaultState);
            
            // Entry Transition.
            List<AnimatorTransition> entryTransitions = new List<AnimatorTransition>();
            foreach (AnimatorTransition transition in source.entryTransitions)
            {
                // 복사.
                AnimatorTransition newTransition = CopyTransitionObject(transition);
                if (!newTransition)
                {
                    Debug.LogError("Entry Transition Copy Failed.");
                    continue;
                }

                // 경로 설정.
                string statePath = GetStatePath(transition.destinationState, source);
                newTransition.destinationState = FindStateToPath(statePath, target);

                statePath = GetStateMachinePath(transition.destinationStateMachine, source);
                newTransition.destinationStateMachine = FindSStateMachineToPath(statePath, target);

                // 조건 복사.
                newTransition.conditions = CopyConditionObjects(transition.conditions);

                // 목록에 추가.
                if (newTransition.destinationState || newTransition.destinationStateMachine)
                {
                    entryTransitions.Add(newTransition);
                    AssetDatabase.AddObjectToAsset(newTransition, target);
                }
            }
            target.entryTransitions = entryTransitions.ToArray();

            // AnyState Tranition.
            List<AnimatorStateTransition> anyStateTransitions = new List<AnimatorStateTransition>();
            foreach (AnimatorStateTransition transition in source.anyStateTransitions)
            {
                // 복사.
                AnimatorStateTransition newTransition = CopyTransitionObject(transition);
                if (!newTransition)
                {
                    Debug.LogError("AnyState Transition Copy Failed.");
                    continue;
                }

                // 경로 설정.
                string statePath = GetStatePath(transition.destinationState, source);
                newTransition.destinationState = FindStateToPath(statePath, target);

                statePath = GetStateMachinePath(transition.destinationStateMachine, source);
                newTransition.destinationStateMachine = FindSStateMachineToPath(statePath, target);

                // 조건 복사.
                newTransition.conditions = CopyConditionObjects(transition.conditions);

                // 목록에 추가.
                if (newTransition.destinationState || newTransition.destinationStateMachine)
                {
                    anyStateTransitions.Add(newTransition);
                    AssetDatabase.AddObjectToAsset(newTransition, target);
                }
            }
            target.anyStateTransitions = anyStateTransitions.ToArray();

            // State Transition 복사.
            for (int i = 0; i < source.states.Length; i++)
            {
                AnimatorState state = source.states[i].state;

                for (int j = 0; j < state.transitions.Length; j++)
                {
                    AnimatorStateTransition transition = state.transitions[j];

                    AnimatorStateTransition newTransition = CopyTransitionObject(state.transitions[j]);

                    // 경로 설정.
                    string statePath = GetStatePath(transition.destinationState, source);
                    newTransition.destinationState = FindStateToPath(statePath, target);

                    statePath = GetStateMachinePath(transition.destinationStateMachine, source);
                    newTransition.destinationStateMachine = FindSStateMachineToPath(statePath, target);

                    // 조건 복사.
                    newTransition.conditions = CopyConditionObjects(transition.conditions);

                    target.states[i].state.AddTransition(newTransition);
                    AssetDatabase.AddObjectToAsset(newTransition, target);
                }
            }

            // StateMachine Transition 복사
            for (int i = 0; i < source.stateMachines.Length; i++)
            {
                AnimatorStateMachine stateMachine = source.stateMachines[i].stateMachine;
                
                AnimatorTransition[] stateMachineTransitions = source.GetStateMachineTransitions(stateMachine);
                for (int j = 0; j < stateMachineTransitions.Length; j++)
                {
                    AnimatorTransition transition = stateMachineTransitions[j];

                    AnimatorState destinationState = transition.destinationState;
                    AnimatorStateMachine destinationStateMachine = transition.destinationStateMachine;                    

                    // 경로 설정.
                    string statePath = GetStatePath(destinationState, source);
                    destinationState = FindStateToPath(statePath, target);

                    statePath = GetStateMachinePath(destinationStateMachine, source);
                    destinationStateMachine = FindSStateMachineToPath(statePath, target);

                    // 조건 복사.
                    AnimatorCondition[] conditions = CopyConditionObjects(transition.conditions);

                    // Transition 추가
                    if (transition.isExit)
                    {
                        AnimatorTransition newTransition = target.AddStateMachineExitTransition(target.stateMachines[i].stateMachine);
                        newTransition.conditions = conditions;
                    }
                    else if (destinationStateMachine != null)
                    {
                        AnimatorTransition newTransition = target.AddStateMachineTransition(target.stateMachines[i].stateMachine, destinationStateMachine);
                        newTransition.conditions = conditions;
                    }
                    else if (destinationState != null)
                    {
                        AnimatorTransition newTransition = target.AddStateMachineTransition(target.stateMachines[i].stateMachine, destinationState);
                        newTransition.conditions = conditions;
                    }
                    else
                    {
                        Debug.LogError("StateMachine Transition 도착점이 없음.");
                    }
                }

            }
        }

        // 블렌드 트리 복사.
        public static BlendTree CopyBlendTree(AnimatorState state, BlendTree blendTree)
        {
            BlendTree newBlendTree = CopyBlendTreeObject(blendTree);

            // 하위 내용 추가.
            for (int i = 0; i < blendTree.children.Length; i++)
            {
                Motion sourceMotion = blendTree.children[i].motion;

                if (sourceMotion.GetType() == typeof(BlendTree))
                {
                    newBlendTree.AddChild(CopyBlendTree(state, (BlendTree)sourceMotion));
                }
                else
                {
                    newBlendTree.AddChild(sourceMotion);
                }
            }

            return newBlendTree;
        }

        // 모든 파라미터 복사.
        public static void CopyAllParameter(RuntimeAnimatorController target, RuntimeAnimatorController source)
        {
            CopyAllParameter((AnimatorController)target, (AnimatorController)source);
        }
        public static void CopyAllParameter(AnimatorController target, AnimatorController source)
        {
            foreach (AnimatorControllerParameter parameter in source.parameters)
            {
                // 이미 있으면 스킵함.
                AnimatorControllerParameter newParamater = FindParameter(target, parameter.name);
                if (newParamater != null)
                    continue;

                // 파라미터 추가.
                newParamater = CopyParameterObject(parameter);
                target.AddParameter(newParamater);
            }
        }
        #endregion

        #region 오브젝트 복사
        // State오브젝트 복사.
        public static AnimatorState CopyStateObject(AnimatorState state)
        {
            if (!state) return null;

            AnimatorState newState = new AnimatorState()
            {
                name = state.name,
                hideFlags = state.hideFlags,
                tag = state.tag,
                speed = state.speed,
                speedParameter = state.speedParameter,
                speedParameterActive = state.speedParameterActive,
                timeParameter = state.timeParameter,
                timeParameterActive = state.timeParameterActive,
                mirror = state.mirror,
                mirrorParameter = state.mirrorParameter,
                mirrorParameterActive = state.mirrorParameterActive,
                cycleOffset = state.cycleOffset,
                cycleOffsetParameter = state.cycleOffsetParameter,
                cycleOffsetParameterActive = state.cycleOffsetParameterActive,
                iKOnFeet = state.iKOnFeet,
                writeDefaultValues = state.writeDefaultValues,
            };

            return newState;
        }

        // 블렌드 트리 오브젝트 복사
        public static BlendTree CopyBlendTreeObject(BlendTree blendTree)
        {
            BlendTree newBlendTree = new BlendTree()
            {
                name = blendTree.name,
                blendType = blendTree.blendType,
                minThreshold = blendTree.minThreshold,
                maxThreshold = blendTree.maxThreshold,
                useAutomaticThresholds = blendTree.useAutomaticThresholds,
                hideFlags = blendTree.hideFlags,
                blendParameter = blendTree.blendParameter,
                blendParameterY = blendTree.blendParameterY
            };

            return newBlendTree;
        }

        // StateBehaviour 복사
        public static StateMachineBehaviour CopyBehaviourObject(StateMachineBehaviour source)
        {
            System.Type type = source.GetType();
            StateMachineBehaviour newBehaviour = (StateMachineBehaviour)StateMachineBehaviour.CreateInstance(type);

            SerializedObject serializedSource = new SerializedObject(source);
            SerializedObject serializedNew = new SerializedObject(newBehaviour);

            SerializedProperty prop = serializedSource.GetIterator();
            while (prop.Next(true))
            {
                serializedNew.CopyFromSerializedProperty(prop);
            }
            serializedNew.ApplyModifiedProperties();

            return newBehaviour;
        }

        // State Transition 복사. (연결대상은 없음.)
        static public AnimatorStateTransition CopyTransitionObject(AnimatorStateTransition transitions)
        {
            AnimatorStateTransition newTransitions = new AnimatorStateTransition
            {
                canTransitionToSelf = transitions.canTransitionToSelf,
                destinationState = null,
                destinationStateMachine = null,
                duration = transitions.duration,
                exitTime = transitions.exitTime,
                hasExitTime = transitions.hasExitTime,
                hasFixedDuration = transitions.hasFixedDuration,
                hideFlags = transitions.hideFlags,
                interruptionSource = transitions.interruptionSource,
                isExit = transitions.isExit,
                mute = transitions.mute,
                name = transitions.name,
                offset = transitions.offset,
                orderedInterruption = transitions.orderedInterruption,
                solo = transitions.solo
            };



            return newTransitions;
        }

        // Transition 복사. (연결대상은 없음.)
        public static AnimatorTransition CopyTransitionObject(AnimatorTransition transitions)
        {
            if (!transitions) return null;

            AnimatorTransition newTransitions = new AnimatorTransition
            {
                destinationState = null,
                destinationStateMachine = null,
                hideFlags = transitions.hideFlags,
                isExit = transitions.isExit,
                mute = transitions.mute,
                name = transitions.name,
                solo = transitions.solo,
            };

            return newTransitions;
        }

        // Condition 복사.
        public static AnimatorCondition CopyConditionObject(AnimatorCondition source)
        {
            AnimatorCondition newCondition = new AnimatorCondition()
            {
                mode = source.mode,
                parameter = source.parameter,
                threshold = source.threshold
            };

            return newCondition;
        }

        // Condition 목록 복사.
        public static AnimatorCondition[] CopyConditionObjects(AnimatorCondition[] source)
        {
            AnimatorCondition[] newConditions = new AnimatorCondition[source.Length];

            for (int i = 0; i < newConditions.Length; i++)
            {
                newConditions[i] = CopyConditionObject(source[i]);
            }

            return newConditions;
        }

        // Paramater 복사.
        public static AnimatorControllerParameter CopyParameterObject(AnimatorControllerParameter parameter)
        {
            if (parameter == null)
                return null;

            AnimatorControllerParameter newParameter = new AnimatorControllerParameter()
            {
                name = parameter.name,
                type = parameter.type,
                defaultBool = parameter.defaultBool,
                defaultFloat = parameter.defaultFloat,
                defaultInt = parameter.defaultInt
            };

            return newParameter;
        }
        #endregion

        #region 가져오기
        // 모든 State 가져오기.
        public static AnimatorState[] GetAllStates(RuntimeAnimatorController controller)
        {
            return GetAllStates((AnimatorController)controller);
        }
        public static AnimatorState[] GetAllStates(AnimatorController controller)
        {
            List<AnimatorState> stateList = new List<AnimatorState>();
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                stateList.AddRange(GetAllStatesRecursive(layer.stateMachine));
            }

            return stateList.ToArray();
        }
        private static AnimatorState[] GetAllStatesRecursive(AnimatorStateMachine stateMachine)
        {
            List<AnimatorState> stateList = new List<AnimatorState>();

            foreach (ChildAnimatorState childState in stateMachine.states)
            {
                stateList.Add(childState.state);
            }

            foreach (ChildAnimatorStateMachine childStateMachine in stateMachine.stateMachines)
            {
                AnimatorStateMachine subStateMachine = childStateMachine.stateMachine;
                stateList.AddRange(GetAllStatesRecursive(subStateMachine));
            }

            return stateList.ToArray();
        }
        #endregion
    }
}