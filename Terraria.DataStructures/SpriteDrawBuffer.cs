using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;

namespace Terraria.DataStructures;

public class SpriteDrawBuffer
{
	private GraphicsDevice graphicsDevice;

	private DynamicVertexBuffer vertexBuffer;

	private IndexBuffer indexBuffer;

	private VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[0];

	private Texture[] textures = new Texture[0];

	private int maxSprites;

	private int vertexCount;

	private VertexBufferBinding[] preBindVertexBuffers;

	private IndexBuffer preBindIndexBuffer;

	public SpriteDrawBuffer(GraphicsDevice graphicsDevice, int defaultSize)
	{
		this.graphicsDevice = graphicsDevice;
		maxSprites = defaultSize;
		CreateBuffers();
	}

	public void CheckGraphicsDevice(GraphicsDevice graphicsDevice)
	{
		if (this.graphicsDevice != graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
			CreateBuffers();
		}
	}

	private void CreateBuffers()
	{
		if (vertexBuffer != null)
		{
			vertexBuffer.Dispose();
		}
		vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), maxSprites * 4, BufferUsage.WriteOnly);
		if (indexBuffer != null)
		{
			indexBuffer.Dispose();
		}
		indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), maxSprites * 6, BufferUsage.WriteOnly);
		indexBuffer.SetData(GenIndexBuffer(maxSprites));
		Array.Resize(ref vertices, maxSprites * 6);
		Array.Resize(ref textures, maxSprites);
	}

	private static ushort[] GenIndexBuffer(int maxSprites)
	{
		ushort[] array = new ushort[maxSprites * 6];
		int num = 0;
		ushort num2 = 0;
		while (num < maxSprites)
		{
			array[num++] = num2;
			array[num++] = (ushort)(num2 + 1);
			array[num++] = (ushort)(num2 + 2);
			array[num++] = (ushort)(num2 + 3);
			array[num++] = (ushort)(num2 + 2);
			array[num++] = (ushort)(num2 + 1);
			num2 = (ushort)(num2 + 4);
		}
		return array;
	}

	public void UploadAndBind()
	{
		if (vertexCount > 0)
		{
			vertexBuffer.SetData(vertices, 0, vertexCount, SetDataOptions.Discard);
		}
		vertexCount = 0;
		Bind();
	}

	public void Bind()
	{
		preBindVertexBuffers = graphicsDevice.GetVertexBuffers();
		preBindIndexBuffer = graphicsDevice.Indices;
		graphicsDevice.SetVertexBuffer(vertexBuffer);
		graphicsDevice.Indices = indexBuffer;
	}

	public void Unbind()
	{
		graphicsDevice.SetVertexBuffers(preBindVertexBuffers);
		graphicsDevice.Indices = preBindIndexBuffer;
		preBindVertexBuffers = null;
		preBindIndexBuffer = null;
	}

	public void DrawRange(int index, int count)
	{
		graphicsDevice.Textures[0] = textures[index];
		graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, index * 4, 0, count * 4, 0, count * 2);
	}

	public void DrawSingle(int index)
	{
		DrawRange(index, 1);
	}

	public void Draw(Texture2D texture, Vector2 position, VertexColors colors)
	{
		Draw(texture, position, null, colors, 0f, Vector2.Zero, 1f, SpriteEffects.None);
	}

	public void Draw(Texture2D texture, Rectangle destination, VertexColors colors)
	{
		Draw(texture, destination, null, colors);
	}

	public void Draw(Texture2D texture, Rectangle destination, Rectangle? sourceRectangle, VertexColors colors)
	{
		Draw(texture, destination, sourceRectangle, colors, 0f, Vector2.Zero, SpriteEffects.None);
	}

	public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, VertexColors color, float rotation, Vector2 origin, float scale, SpriteEffects effects)
	{
		Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale, scale), effects);
	}

	public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, VertexColors colors, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
	{
		float num880;
		float wall3;
		if (sourceRectangle.HasValue)
		{
			num880 = (float)sourceRectangle.Value.Width * scale.X;
			wall3 = (float)sourceRectangle.Value.Height * scale.Y;
		}
		else
		{
			num880 = (float)texture.Width * scale.X;
			wall3 = (float)texture.Height * scale.Y;
		}
		Draw(texture, new Vector4(position.X, position.Y, num880, wall3), sourceRectangle, colors, rotation, origin, effects, 0f);
	}

	public void Draw(Texture2D texture, Rectangle destination, Rectangle? sourceRectangle, VertexColors colors, float rotation, Vector2 origin, SpriteEffects effects)
	{
		Draw(texture, new Vector4(destination.X, destination.Y, destination.Width, destination.Height), sourceRectangle, colors, rotation, origin, effects, 0f);
	}

	public void Draw(Texture2D texture, Vector4 destinationRectangle, Rectangle? sourceRectangle, VertexColors colors, float rotation, Vector2 origin, SpriteEffects effect, float depth)
	{
		Vector4 num872 = default(Vector4);
		if (sourceRectangle.HasValue)
		{
			num872.X = sourceRectangle.Value.X;
			num872.Y = sourceRectangle.Value.Y;
			num872.Z = sourceRectangle.Value.Width;
			num872.W = sourceRectangle.Value.Height;
		}
		else
		{
			num872.X = 0f;
			num872.Y = 0f;
			num872.Z = texture.Width;
			num872.W = texture.Height;
		}
		Vector2 num873 = default(Vector2);
		num873.X = num872.X / (float)texture.Width;
		num873.Y = num872.Y / (float)texture.Height;
		Vector2 texCoordBR = default(Vector2);
		texCoordBR.X = (num872.X + num872.Z) / (float)texture.Width;
		texCoordBR.Y = (num872.Y + num872.W) / (float)texture.Height;
		if ((effect & SpriteEffects.FlipVertically) != 0)
		{
			float y = texCoordBR.Y;
			texCoordBR.Y = num873.Y;
			num873.Y = y;
		}
		if ((effect & SpriteEffects.FlipHorizontally) != 0)
		{
			float x = texCoordBR.X;
			texCoordBR.X = num873.X;
			num873.X = x;
		}
		QueueSprite(destinationRectangle, -origin, colors, num872, num873, texCoordBR, texture, depth, rotation);
	}

	private void QueueSprite(Vector4 destinationRect, Vector2 origin, VertexColors colors, Vector4 sourceRectangle, Vector2 texCoordTL, Vector2 texCoordBR, Texture2D texture, float depth, float rotation)
	{
		float num866 = origin.X / sourceRectangle.Z;
		float num872 = origin.Y / sourceRectangle.W;
		float num867 = destinationRect.X;
		float num868 = destinationRect.Y;
		float num869 = destinationRect.Z;
		float num870 = destinationRect.W;
		float num871 = num866 * num869;
		float num873 = num872 * num870;
		float num874;
		float num875;
		if (rotation != 0f)
		{
			num874 = (float)Math.Cos(rotation);
			num875 = (float)Math.Sin(rotation);
		}
		else
		{
			num874 = 1f;
			num875 = 0f;
		}
		if (vertexCount + 4 >= maxSprites * 4)
		{
			maxSprites *= 2;
			CreateBuffers();
		}
		textures[vertexCount / 4] = texture;
		PushVertex(new Vector3(num867 + num871 * num874 - num873 * num875, num868 + num871 * num875 + num873 * num874, depth), colors.TopLeftColor, texCoordTL);
		PushVertex(new Vector3(num867 + (num871 + num869) * num874 - num873 * num875, num868 + (num871 + num869) * num875 + num873 * num874, depth), colors.TopRightColor, new Vector2(texCoordBR.X, texCoordTL.Y));
		PushVertex(new Vector3(num867 + num871 * num874 - (num873 + num870) * num875, num868 + num871 * num875 + (num873 + num870) * num874, depth), colors.BottomLeftColor, new Vector2(texCoordTL.X, texCoordBR.Y));
		PushVertex(new Vector3(num867 + (num871 + num869) * num874 - (num873 + num870) * num875, num868 + (num871 + num869) * num875 + (num873 + num870) * num874, depth), colors.BottomRightColor, texCoordBR);
	}

	private void PushVertex(Vector3 pos, Color color, Vector2 texCoord)
	{
		SetVertex(ref vertices[vertexCount++], pos, color, texCoord);
	}

	private static void SetVertex(ref VertexPositionColorTexture vertex, Vector3 pos, Color color, Vector2 texCoord)
	{
		vertex.Position = pos;
		vertex.Color = color;
		vertex.TextureCoordinate = texCoord;
	}
}
