using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public readonly struct TileType
{
	public int id { get; init; }
	public string name {  get; init; }
	public int cost { get; init; }
	public bool isEncounter { get; init; }

	public TileType(int _id, string _name, int _cost, bool _isEncounter = false)
	{
		id = _id;
		name = _name;
		cost = _cost;
		isEncounter = _isEncounter;
	}
}


public class Location
{
	public Vector2 coords;
	public TileType tileType;
	public int scoreF;
	public int scoreG;
	public int scoreH;
	public Location? Parent;

	public Location(Vector2 _coords, int _tileGid = 0)
	{
		coords = _coords;
		foreach (TileType item in ServiceLocator.GetService<Map>().tileTypesList)
        {
			if (item.id == _tileGid)
				tileType = item;
        }
	}

	public static float ComputeHScore(Location _source, Location _target)
	{
		return Math.Abs(_target.coords.X - _source.coords.X) + Math.Abs(_target.coords.Y - _source.coords.Y);
	}
}