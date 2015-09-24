using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
/// <summary>
/// This script creates a destructible pixel based terrain.
/// All credit goes to Jared Counts, who wrote the code this
/// script is based on.
/// 
/// Source: http://gamedevelopment.tutsplus.com/tutorials/coding-destructible-pixel-terrain--gamedev-45
/// 
/// </summary>
public class DestructibleTerrain : MonoBehaviour {

    public Texture2D sourceTexture;
    public int destructionResolution = 3;

    private Texture2D texture;
    private Color32[] pixels;

    void Awake() {

        SetupTextureClone();

    }

    /// <summary>
    /// Used to create a backup of the original texture,
    /// so that it's not affected by this script.
    /// </summary>
    void SetupTextureClone()
    {

        if (sourceTexture)
        {

            //Get the component
            pixels = sourceTexture.GetPixels32();
            texture = new Texture2D(sourceTexture.width, sourceTexture.height);
            texture.SetPixels32(pixels);
            texture.Apply();

            GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture,
                          new Rect(0, 0, texture.width, texture.height),
                          new Vector2(0f, 0f));

        }

    }

    float[] CalculateSurfaceNormals(int x, int y)
    {

        float avgX = 0f;
        float avgY = 0f;

        for (int w = -1; w <= 3; w++)
        {
            for (int h = -3; h <= 3; h++)
            {
                if(IsPixelSolid(x + w, y + h)) {
                    avgX -= w;
                    avgY -= h;
                }
            }
        }

        float len = Mathf.Sqrt(avgX * avgX + avgY * avgY);
        return new float[]{avgX/len,avgY/len};

    }

    bool IsPixelSolid(int x, int y)
    {

        //TODO: Check why he skipped the border
        if (x > 0 && x < texture.width &&
           y > 0 && y < texture.height)
        {
            return texture.GetPixel(x,y).a != 0f;
        }
        else
        {
            return false;
        }
    }

    void RemovePixel(int x, int y)
    {
        //Remove colour from the pixel
        //print("Hello");
        texture.SetPixel(x, y, new Color(0, 0, 0, 0));
    }

    public void Explosion(int xPos, int yPos, float radius)
    {

        float radiusSq = radius * radius;

        //Loop through every x from xPos-radius to xPos+radius
        for(int x = xPos - (int)radius; x < xPos + (int)radius; x += destructionResolution)
        {

            //Ensure we inside the terrain
            if (x > 0 && x < texture.width)
            {

                for (int y = yPos - (int)radius; y < yPos + (int)radius; y += destructionResolution)
                {

                    if (y > 0 && y < texture.height)
                    {

                        //first determine if these pixels are solid
                        //int solidx = 0, solidy = 0;
                        bool solid = false;

                        //loop through every pixel from (xpos,ypos) to (xpos + destructionres, ypos + destructionres)
                        //to find whether this area is solid or not
                        for (int i = 0; i < destructionResolution && !solid; i++)
                        {
                            for (int j = 0; j < destructionResolution && !solid; j++)
                            {
                                if (IsPixelSolid(x + i, y + j))
                                {
                                    solid = true;
                                    //solidx = x + i;
                                    //solidy = y + j;
                                }
                            }
                        }

                        //now it's solid, we need to find if it's in range
                        if (solid)
                        {
                            float xdiff = x - xPos;
                            float ydiff = y - yPos;
                            float distsq = xdiff * xdiff + ydiff * ydiff;

                            if (distsq < radiusSq)
                            {

                                //destroy the pixels
                                for (int i = 0; i < destructionResolution; i++)
                                {
                                    for (int j = 0; j < destructionResolution; j++)
                                    {
                                        RemovePixel(x + i, y + j);
                                        
                                    }
                                }

                            }

                        }

                    }

                }

            }

        }

        texture.Apply();

    }
	
}
