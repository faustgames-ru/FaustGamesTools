package com.FaustGames.Core.Rendering.Effects;

import android.content.Context;
import com.FaustGames.Asteroids3dFree.R;
import com.FaustGames.Core.Mathematics.Matrix;
import com.FaustGames.Core.Mathematics.Matrix3;
import com.FaustGames.Core.Mathematics.Vertex;
import com.FaustGames.Core.Rendering.Color;
import com.FaustGames.Core.Rendering.Effects.Attributes.*;
import com.FaustGames.Core.Rendering.Textures.Texture;

public class #EffectName extends Effect {

    public #EffectName() {
        super("", "",
                new String[]{
						#foreachUniform
                        "#UniformName",
						#endForeach
                },
                new String[]{
						#foreachAttribute
                        "#AttributeName",
						#endForeach
                });
    }

    @Override
    public void onCreate(Context context) {
        super.onCreate(context);
        super.onCreate(context);
        setCode(
                loadFromRaw(context, R.raw.#EffectFileNameWithoutExtension),
                loadFromRaw(context, R.raw.#EffectFileNameWithoutExtension));
    }

    public void set(
		#foreachUniform
		#UniformType #uniformName #,
		#endForeach
	){
		#foreachUniform
        Parameters.get(#uniformIndex).setValue(#uniformName);
		#endForeach
	}

	#foreachUniform
    public void set#UniformName(#UniformType value){
        Parameters.get(#uniformIndex).setValue(#uniformName);
	}
	#endForeach
}
