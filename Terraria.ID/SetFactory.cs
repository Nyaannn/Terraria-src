using System;
using System.Collections.Generic;

namespace Terraria.ID;

public class SetFactory
{
	protected int _size;

	private readonly Queue<int[]> _intBufferCache = new Queue<int[]>();

	private readonly Queue<ushort[]> _ushortBufferCache = new Queue<ushort[]>();

	private readonly Queue<bool[]> _boolBufferCache = new Queue<bool[]>();

	private readonly Queue<float[]> _floatBufferCache = new Queue<float[]>();

	private object _queueLock = new object();

	public SetFactory(int size)
	{
		if (size == 0)
		{
			throw new ArgumentOutOfRangeException("size cannot be 0, the intializer for Count must run first");
		}
		_size = size;
	}

	protected bool[] GetBoolBuffer()
	{
		lock (_queueLock)
		{
			if (_boolBufferCache.Count == 0)
			{
				return new bool[_size];
			}
			return _boolBufferCache.Dequeue();
		}
	}

	protected int[] GetIntBuffer()
	{
		lock (_queueLock)
		{
			if (_intBufferCache.Count == 0)
			{
				return new int[_size];
			}
			return _intBufferCache.Dequeue();
		}
	}

	protected ushort[] GetUshortBuffer()
	{
		lock (_queueLock)
		{
			if (_ushortBufferCache.Count == 0)
			{
				return new ushort[_size];
			}
			return _ushortBufferCache.Dequeue();
		}
	}

	protected float[] GetFloatBuffer()
	{
		lock (_queueLock)
		{
			if (_floatBufferCache.Count == 0)
			{
				return new float[_size];
			}
			return _floatBufferCache.Dequeue();
		}
	}

	public void Recycle<T>(T[] buffer)
	{
		lock (_queueLock)
		{
			if (typeof(T).Equals(typeof(bool)))
			{
				_boolBufferCache.Enqueue((bool[])(object)buffer);
			}
			else if (typeof(T).Equals(typeof(int)))
			{
				_intBufferCache.Enqueue((int[])(object)buffer);
			}
		}
	}

	public bool[] CreateBoolSet(params int[] types)
	{
		return CreateBoolSet(defaultState: false, types);
	}

	public bool[] CreateBoolSet(bool defaultState, params int[] types)
	{
		bool[] boolBuffer = GetBoolBuffer();
		for (int i = 0; i < boolBuffer.Length; i++)
		{
			boolBuffer[i] = defaultState;
		}
		for (int j = 0; j < types.Length; j++)
		{
			boolBuffer[types[j]] = !defaultState;
		}
		return boolBuffer;
	}

	public int[] CreateIntSet(params int[] types)
	{
		return CreateIntSet(-1, types);
	}

	public int[] CreateIntSet(int defaultState, params int[] inputs)
	{
		if (inputs.Length % 2 != 0)
		{
			throw new Exception("You have a bad length for inputs on CreateArraySet");
		}
		int[] num = GetIntBuffer();
		for (int num2 = 0; num2 < num.Length; num2++)
		{
			num[num2] = defaultState;
		}
		for (int num3 = 0; num3 < inputs.Length; num3 += 2)
		{
			num[inputs[num3]] = inputs[num3 + 1];
		}
		return num;
	}

	public ushort[] CreateUshortSet(ushort defaultState, params ushort[] inputs)
	{
		if (inputs.Length % 2 != 0)
		{
			throw new Exception("You have a bad length for inputs on CreateArraySet");
		}
		ushort[] publicityOptions = GetUshortBuffer();
		for (int i = 0; i < publicityOptions.Length; i++)
		{
			publicityOptions[i] = defaultState;
		}
		for (int j = 0; j < inputs.Length; j += 2)
		{
			publicityOptions[inputs[j]] = inputs[j + 1];
		}
		return publicityOptions;
	}

	public float[] CreateFloatSet(float defaultState, params float[] inputs)
	{
		if (inputs.Length % 2 != 0)
		{
			throw new Exception("You have a bad length for inputs on CreateArraySet");
		}
		float[] uIElement = GetFloatBuffer();
		for (int i = 0; i < uIElement.Length; i++)
		{
			uIElement[i] = defaultState;
		}
		for (int j = 0; j < inputs.Length; j += 2)
		{
			uIElement[(int)inputs[j]] = inputs[j + 1];
		}
		return uIElement;
	}

	public T[] CreateCustomSet<T>(T defaultState, params object[] inputs)
	{
		if (inputs.Length % 2 != 0)
		{
			throw new Exception("You have a bad length for inputs on CreateCustomSet");
		}
		T[] uIElement = new T[_size];
		for (int num = 0; num < uIElement.Length; num++)
		{
			uIElement[num] = defaultState;
		}
		if (inputs != null)
		{
			for (int uIImage = 0; uIImage < inputs.Length; uIImage += 2)
			{
				T element = (typeof(T).IsPrimitive ? ((T)inputs[uIImage + 1]) : ((typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)) ? ((T)inputs[uIImage + 1]) : ((!typeof(T).IsClass) ? ((T)Convert.ChangeType(inputs[uIImage + 1], typeof(T))) : ((T)inputs[uIImage + 1]))));
				if (inputs[uIImage] is ushort)
				{
					uIElement[(ushort)inputs[uIImage]] = element;
				}
				else if (inputs[uIImage] is int)
				{
					uIElement[(int)inputs[uIImage]] = element;
				}
				else
				{
					uIElement[(short)inputs[uIImage]] = element;
				}
			}
		}
		return uIElement;
	}
}
