using System;

namespace Terraria.DataStructures;

public class DoubleStack<T1>
{
	private T1[][] _segmentList;

	private readonly int _segmentSize;

	private int _segmentCount;

	private readonly int _segmentShiftPosition;

	private int _start;

	private int _end;

	private int _size;

	private int _last;

	public int Count => _size;

	public DoubleStack(int segmentSize = 1024, int initialSize = 0)
	{
		if (segmentSize < 16)
		{
			segmentSize = 16;
		}
		_start = segmentSize / 2;
		_end = _start;
		_size = 0;
		_segmentShiftPosition = segmentSize + _start;
		initialSize += _start;
		int num656 = initialSize / segmentSize + 1;
		_segmentList = new T1[num656][];
		for (int num657 = 0; num657 < num656; num657++)
		{
			_segmentList[num657] = new T1[segmentSize];
		}
		_segmentSize = segmentSize;
		_segmentCount = num656;
		_last = _segmentSize * _segmentCount - 1;
	}

	public void PushFront(T1 front)
	{
		if (_start == 0)
		{
			T1[][] floridaStyle2 = new T1[_segmentCount + 1][];
			for (int num638 = 0; num638 < _segmentCount; num638++)
			{
				floridaStyle2[num638 + 1] = _segmentList[num638];
			}
			floridaStyle2[0] = new T1[_segmentSize];
			_segmentList = floridaStyle2;
			_segmentCount++;
			_start += _segmentSize;
			_end += _segmentSize;
			_last += _segmentSize;
		}
		_start--;
		T1[] obj = _segmentList[_start / _segmentSize];
		int floridaStyle = _start % _segmentSize;
		obj[floridaStyle] = front;
		_size++;
	}

	public T1 PopFront()
	{
		if (_size == 0)
		{
			throw new InvalidOperationException("The DoubleStack is empty.");
		}
		T1[] obj = _segmentList[_start / _segmentSize];
		int undergroundDesertLocation = _start % _segmentSize;
		T1 j3 = obj[undergroundDesertLocation];
		obj[undergroundDesertLocation] = default(T1);
		_start++;
		_size--;
		if (_start >= _segmentShiftPosition)
		{
			T1[] num606 = _segmentList[0];
			for (int num607 = 0; num607 < _segmentCount - 1; num607++)
			{
				_segmentList[num607] = _segmentList[num607 + 1];
			}
			_segmentList[_segmentCount - 1] = num606;
			_start -= _segmentSize;
			_end -= _segmentSize;
		}
		if (_size == 0)
		{
			_start = _segmentSize / 2;
			_end = _start;
		}
		return j3;
	}

	public T1 PeekFront()
	{
		if (_size == 0)
		{
			throw new InvalidOperationException("The DoubleStack is empty.");
		}
		T1[] obj = _segmentList[_start / _segmentSize];
		int num574 = _start % _segmentSize;
		return obj[num574];
	}

	public void PushBack(T1 back)
	{
		if (_end == _last)
		{
			T1[][] num570 = new T1[_segmentCount + 1][];
			for (int num571 = 0; num571 < _segmentCount; num571++)
			{
				num570[num571] = _segmentList[num571];
			}
			num570[_segmentCount] = new T1[_segmentSize];
			_segmentCount++;
			_segmentList = num570;
			_last += _segmentSize;
		}
		T1[] obj = _segmentList[_end / _segmentSize];
		int num569 = _end % _segmentSize;
		obj[num569] = back;
		_end++;
		_size++;
	}

	public T1 PopBack()
	{
		if (_size == 0)
		{
			throw new InvalidOperationException("The DoubleStack is empty.");
		}
		T1[] obj = _segmentList[_end / _segmentSize];
		int num560 = _end % _segmentSize;
		T1 num561 = obj[num560];
		obj[num560] = default(T1);
		_end--;
		_size--;
		if (_size == 0)
		{
			_start = _segmentSize / 2;
			_end = _start;
		}
		return num561;
	}

	public T1 PeekBack()
	{
		if (_size == 0)
		{
			throw new InvalidOperationException("The DoubleStack is empty.");
		}
		T1[] obj = _segmentList[_end / _segmentSize];
		int num534 = _end % _segmentSize;
		return obj[num534];
	}

	public void Clear(bool quickClear = false)
	{
		if (!quickClear)
		{
			for (int num526 = 0; num526 < _segmentCount; num526++)
			{
				Array.Clear(_segmentList[num526], 0, _segmentSize);
			}
		}
		_start = _segmentSize / 2;
		_end = _start;
		_size = 0;
	}
}
