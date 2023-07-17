using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders;

public class TextureHelper
{
    private GraphicsDevice? _device; 
    private static TextureHelper? _instance;
    private Texture2D? _dynamicTexture;
    
    public static TextureHelper Current => _instance ??= new TextureHelper();

    public void Initialize(GraphicsDevice device)
    {
        _device = device;
    }

    public Texture2D CreateTexture()
    {
        if (_dynamicTexture == null)
        {
            _dynamicTexture = new Texture2D(_device, 1, 1);
            _dynamicTexture.SetData<Color>(new Color[] { Color.White });
        }
        return _dynamicTexture;
    }
    
}