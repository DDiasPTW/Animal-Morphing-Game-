using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Cyan
{
	public class Blit : ScriptableRendererFeature
	{
		[Serializable]
		public class BlitSettings
		{
			public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
			public Material blitMaterial = null;
			public int blitMaterialPassIndex = 0;
			public bool setInverseViewMatrix = false;
			public bool requireDepthNormals = false;
			public Target srcType = Target.CameraColor;
			public string srcTextureId = "_CameraColorTexture";
			public RenderTexture srcTextureObject;
			public Target dstType = Target.CameraColor;
			public string dstTextureId = "_BlitPassTexture";
			public RenderTexture dstTextureObject;
			public bool overrideGraphicsFormat = false;
			public UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat;
		}

		public enum Target
		{
			CameraColor,
			TextureID,
			RenderTextureObject
		}

		public BlitSettings settings = new BlitSettings();
		private BlitPass blitPass;

		public override void Create()
		{
			blitPass = new BlitPass(settings, name);
			if (settings.graphicsFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None)
			{
				settings.graphicsFormat = SystemInfo.GetGraphicsFormat(UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
			}
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (settings.blitMaterial == null)
			{
				Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
				return;
			}
			blitPass.Setup(renderer, settings);
			renderer.EnqueuePass(blitPass);
		}

		class BlitPass : ScriptableRenderPass
		{
			private Material blitMaterial = null;
			private BlitSettings settings;
			private string profilerTag;
			private RTHandle m_TemporaryColorTexture;
			private RTHandle m_DestinationTexture;
			private bool m_TemporaryColorTextureAllocated = false;
			private bool m_DestinationTextureAllocated = false;



			public BlitPass(BlitSettings settings, string tag)
			{
				this.settings = settings;
				this.blitMaterial = settings.blitMaterial;
				this.profilerTag = tag;
				this.renderPassEvent = settings.Event;

				// m_TemporaryColorTexture = RTHandles.Alloc(Vector2.one, name: "_TemporaryColorTexture");
				// if (settings.dstType == Target.TextureID) {
				//     m_DestinationTexture = RTHandles.Alloc(Vector2.one, name: settings.dstTextureId);
				// }
			}

			public void Setup(ScriptableRenderer renderer, BlitSettings settings)
			{
				this.settings = settings;
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

				// Allocate RTHandles on demand
				if (!m_TemporaryColorTextureAllocated)
				{
					m_TemporaryColorTexture = RTHandles.Alloc(Vector2.one, name: "_TemporaryColorTexture");
					m_TemporaryColorTextureAllocated = true;
				}
				if (settings.dstType == Target.TextureID && !m_DestinationTextureAllocated)
				{
					m_DestinationTexture = RTHandles.Alloc(Vector2.one, name: settings.dstTextureId);
					m_DestinationTextureAllocated = true;
				}


				if (settings.setInverseViewMatrix)
				{
					Shader.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
				}

				RTHandle source = ResolveSource(ref renderingData);
				RTHandle destination = ResolveDestination();

				if (source == null || destination == null)
				{
					Debug.LogError("BlitPass source or destination not resolved correctly.");
					return;
				}

				Blit(cmd, source, destination);
				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}

			private RTHandle ResolveSource(ref RenderingData renderingData)
			{
				switch (settings.srcType)
				{
					case Target.CameraColor:
						return renderingData.cameraData.renderer.cameraColorTargetHandle;
					case Target.TextureID:
						// Implement your logic here if you need to resolve a texture by ID
						return null;
					case Target.RenderTextureObject:
						return RTHandles.Alloc(settings.srcTextureObject);
					default:
						return null;
				}
			}

			private RTHandle ResolveDestination()
			{
				switch (settings.dstType)
				{
					case Target.CameraColor:
						// This assumes you want to blit to the camera's target. Adjust as necessary.
						return RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget);
					case Target.TextureID:
						return m_DestinationTexture;
					case Target.RenderTextureObject:
						return RTHandles.Alloc(settings.dstTextureObject);
					default:
						return null;
				}
			}

			private void Blit(CommandBuffer cmd, RTHandle source, RTHandle destination)
			{
				// Adjust as necessary. This is a simplified example.
				// You may need to use a material and handle blitting manually if you're doing something more complex.
				cmd.SetRenderTarget(destination);
				cmd.Blit(source, destination, blitMaterial, settings.blitMaterialPassIndex);
			}

			public override void FrameCleanup(CommandBuffer cmd)
			{
				base.FrameCleanup(cmd);
				// Properly release RTHandles to avoid the assertion failure
				if (m_TemporaryColorTextureAllocated)
				{
					m_TemporaryColorTexture.Release();
					m_TemporaryColorTextureAllocated = false;
				}
				if (m_DestinationTextureAllocated)
				{
					m_DestinationTexture.Release();
					m_DestinationTextureAllocated = false;
				}
			}
		}
	}
}
