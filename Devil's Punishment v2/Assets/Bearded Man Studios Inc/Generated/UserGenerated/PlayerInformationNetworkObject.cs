using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0,0]")]
	public partial class PlayerInformationNetworkObject : NetworkObject
	{
		public const int IDENTITY = 7;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector3 _Position;
		public event FieldEvent<Vector3> PositionChanged;
		public InterpolateVector3 PositionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 Position
		{
			get { return _Position; }
			set
			{
				// Don't do anything if the value is the same
				if (_Position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_Position = value;
				hasDirtyFields = true;
			}
		}

		public void SetPositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_Position(ulong timestep)
		{
			if (PositionChanged != null) PositionChanged(_Position, timestep);
			if (fieldAltered != null) fieldAltered("Position", _Position, timestep);
		}
		[ForgeGeneratedField]
		private Quaternion _Rotation;
		public event FieldEvent<Quaternion> RotationChanged;
		public InterpolateQuaternion RotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion Rotation
		{
			get { return _Rotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_Rotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_Rotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetRotationDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_Rotation(ulong timestep)
		{
			if (RotationChanged != null) RotationChanged(_Rotation, timestep);
			if (fieldAltered != null) fieldAltered("Rotation", _Rotation, timestep);
		}
		[ForgeGeneratedField]
		private int _NetworkID;
		public event FieldEvent<int> NetworkIDChanged;
		public Interpolated<int> NetworkIDInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int NetworkID
		{
			get { return _NetworkID; }
			set
			{
				// Don't do anything if the value is the same
				if (_NetworkID == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_NetworkID = value;
				hasDirtyFields = true;
			}
		}

		public void SetNetworkIDDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_NetworkID(ulong timestep)
		{
			if (NetworkIDChanged != null) NetworkIDChanged(_NetworkID, timestep);
			if (fieldAltered != null) fieldAltered("NetworkID", _NetworkID, timestep);
		}
		[ForgeGeneratedField]
		private int _Animation;
		public event FieldEvent<int> AnimationChanged;
		public Interpolated<int> AnimationInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int Animation
		{
			get { return _Animation; }
			set
			{
				// Don't do anything if the value is the same
				if (_Animation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_Animation = value;
				hasDirtyFields = true;
			}
		}

		public void SetAnimationDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_Animation(ulong timestep)
		{
			if (AnimationChanged != null) AnimationChanged(_Animation, timestep);
			if (fieldAltered != null) fieldAltered("Animation", _Animation, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			PositionInterpolation.current = PositionInterpolation.target;
			RotationInterpolation.current = RotationInterpolation.target;
			NetworkIDInterpolation.current = NetworkIDInterpolation.target;
			AnimationInterpolation.current = AnimationInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _Position);
			UnityObjectMapper.Instance.MapBytes(data, _Rotation);
			UnityObjectMapper.Instance.MapBytes(data, _NetworkID);
			UnityObjectMapper.Instance.MapBytes(data, _Animation);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_Position = UnityObjectMapper.Instance.Map<Vector3>(payload);
			PositionInterpolation.current = _Position;
			PositionInterpolation.target = _Position;
			RunChange_Position(timestep);
			_Rotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			RotationInterpolation.current = _Rotation;
			RotationInterpolation.target = _Rotation;
			RunChange_Rotation(timestep);
			_NetworkID = UnityObjectMapper.Instance.Map<int>(payload);
			NetworkIDInterpolation.current = _NetworkID;
			NetworkIDInterpolation.target = _NetworkID;
			RunChange_NetworkID(timestep);
			_Animation = UnityObjectMapper.Instance.Map<int>(payload);
			AnimationInterpolation.current = _Animation;
			AnimationInterpolation.target = _Animation;
			RunChange_Animation(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Rotation);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _NetworkID);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Animation);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (PositionInterpolation.Enabled)
				{
					PositionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					PositionInterpolation.Timestep = timestep;
				}
				else
				{
					_Position = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_Position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (RotationInterpolation.Enabled)
				{
					RotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RotationInterpolation.Timestep = timestep;
				}
				else
				{
					_Rotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_Rotation(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (NetworkIDInterpolation.Enabled)
				{
					NetworkIDInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					NetworkIDInterpolation.Timestep = timestep;
				}
				else
				{
					_NetworkID = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_NetworkID(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (AnimationInterpolation.Enabled)
				{
					AnimationInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					AnimationInterpolation.Timestep = timestep;
				}
				else
				{
					_Animation = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_Animation(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (PositionInterpolation.Enabled && !PositionInterpolation.current.UnityNear(PositionInterpolation.target, 0.0015f))
			{
				_Position = (Vector3)PositionInterpolation.Interpolate();
				//RunChange_Position(PositionInterpolation.Timestep);
			}
			if (RotationInterpolation.Enabled && !RotationInterpolation.current.UnityNear(RotationInterpolation.target, 0.0015f))
			{
				_Rotation = (Quaternion)RotationInterpolation.Interpolate();
				//RunChange_Rotation(RotationInterpolation.Timestep);
			}
			if (NetworkIDInterpolation.Enabled && !NetworkIDInterpolation.current.UnityNear(NetworkIDInterpolation.target, 0.0015f))
			{
				_NetworkID = (int)NetworkIDInterpolation.Interpolate();
				//RunChange_NetworkID(NetworkIDInterpolation.Timestep);
			}
			if (AnimationInterpolation.Enabled && !AnimationInterpolation.current.UnityNear(AnimationInterpolation.target, 0.0015f))
			{
				_Animation = (int)AnimationInterpolation.Interpolate();
				//RunChange_Animation(AnimationInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerInformationNetworkObject() : base() { Initialize(); }
		public PlayerInformationNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerInformationNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
