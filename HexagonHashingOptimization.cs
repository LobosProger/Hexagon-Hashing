using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class HexagonHashingOptimization : MonoBehaviour
{
	//* Main variables for rebuilding
	//* => They need to rebuild separate parts - simple components or network
	//[SerializeField]
	private bool visibleByLocalPlayer;
	//[SerializeField]
	private bool visibleBySomeNetworkedPlayer;

	//* We need these values as flaqs to check, if has some changes
	//* => So, just not complete rebuilding every frame, we check changes by these variables
	private bool visibleByLocalPlayerRebuild;
	private bool visibleBySomeNetworkedPlayerRebuild;

	//* Object type - Simple or Networked
	//* => If networked type - we just turning off networked components - scripts, etc
	//* => If simple type - we just turning off simple components - animators, canvases, etc
	public enum TypeOfObject
	{
		Simple,
		NetworkedObject
	}
	public TypeOfObject typeOfObject;

	[Space]
	//* Also we need to optimize animators
	[SerializeField]
	private Animator[] AnimatorsOptimize;

	//* Below components need for optimization
	//* => We just assign variables of them from inspector
	[SerializeField]
	private UnityEvent<bool> OnRebuildComponents;

	//* This is array for network components (from bots and etc)
	//* => We just turn off network scripts to stop simulation of game world (for optimization)
	[SerializeField]
	private UnityEvent<bool> OnRebuildNetworkComponents;

	//* If object does not moves, like static, we just use ready key
	[HideInInspector]
	public Vector2Int key;

	//* Firstly, if we have Animator components - keep their state on disable to avoid strange bugs
	//* Then, we are adding our script into global HexagonHashing component
	private void Awake()
	{
		//* Если игра запускается впервые, то просто ребьюлдим наши компоненты
		if (!HexagonHashing.isInitialized)
		{
			RebuildComponents();
			RebuildNetworkComponents();
		}
		else
		{
			//* Если же игра уже запущена и это появляется например, пуля посредством использования функции Instatiate, то ребьюлдинг в реальном времени требуется
			//* Т.к. у главного скрипта есть определенное время работы (интервалы перед запуском функции ребьюлдинга) и может получится так, что если мы будем использовать
			//* обычные функции ребьюлдинга, то в какой-то момент игрок заметит, что например анимация объекта воспроизвелась, потом остановилась, потом снова воспроизвелась
			HexagonHashing.singletonHexagon.RebuildEntity(this);
		}

		HexagonHashing.allHexagonOptimizationObjects.Add(this);
	}

	private void OnDestroy() =>
		HexagonHashing.allHexagonOptimizationObjects.Remove(this);


	public void Rebuild(bool localPlayer, bool someNetworkedPlayer)
	{
		visibleByLocalPlayer = localPlayer;
		visibleBySomeNetworkedPlayer = someNetworkedPlayer;

		if (visibleByLocalPlayer != visibleByLocalPlayerRebuild)
			RebuildComponents();

		if (visibleBySomeNetworkedPlayer != visibleBySomeNetworkedPlayerRebuild || visibleByLocalPlayer != visibleByLocalPlayerRebuild)
			RebuildNetworkComponents();
	}

	private void RebuildComponents()
	{
		visibleByLocalPlayerRebuild = visibleByLocalPlayer;

		RebuildAnimatorArray(AnimatorsOptimize, visibleByLocalPlayer);
		OnRebuildComponents.Invoke(visibleByLocalPlayer);
	}

	private void RebuildNetworkComponents()
	{
		visibleBySomeNetworkedPlayerRebuild = visibleBySomeNetworkedPlayer;

		if (NetworkServer.active && typeOfObject == TypeOfObject.NetworkedObject)
			OnRebuildNetworkComponents.Invoke(visibleByLocalPlayer);
	}

	private void RebuildAnimatorArray(Animator[] entry, bool visible)
	{
		foreach (Animator everyEntry in entry)
		{
			if (everyEntry != null)
			{
				everyEntry.enabled = visible;
				if (visible)
				{
					//Debug.Log("Time: " + (float)NetworkTime.time);
					everyEntry.Play(everyEntry.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, ((float)NetworkTime.time % everyEntry.GetCurrentAnimatorStateInfo(0).length) / everyEntry.GetCurrentAnimatorStateInfo(0).length);//everyEntry.GetCurrentAnimatorStateInfo(0).normalizedTime = (Time.time % everyEntry.GetCurrentAnimatorStateInfo(0).length) / everyEntry.GetCurrentAnimatorStateInfo(0).length;
				}
			}
		}
	}
}
