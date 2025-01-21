// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial
// declarations in another file.
// </auto-generated>
#pragma warning disable 0109
#pragma warning disable 1591


namespace Quantum.Prototypes {
  using Photon.Deterministic;
  using Quantum;
  using Quantum.Core;
  using Quantum.Collections;
  using Quantum.Inspector;
  using Quantum.Physics2D;
  using Quantum.Physics3D;
  using Byte = System.Byte;
  using SByte = System.SByte;
  using Int16 = System.Int16;
  using UInt16 = System.UInt16;
  using Int32 = System.Int32;
  using UInt32 = System.UInt32;
  using Int64 = System.Int64;
  using UInt64 = System.UInt64;
  using Boolean = System.Boolean;
  using String = System.String;
  using Object = System.Object;
  using FlagsAttribute = System.FlagsAttribute;
  using SerializableAttribute = System.SerializableAttribute;
  using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
  using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
  using FieldOffsetAttribute = System.Runtime.InteropServices.FieldOffsetAttribute;
  using StructLayoutAttribute = System.Runtime.InteropServices.StructLayoutAttribute;
  using LayoutKind = System.Runtime.InteropServices.LayoutKind;
  #if QUANTUM_UNITY //;
  using TooltipAttribute = UnityEngine.TooltipAttribute;
  using HeaderAttribute = UnityEngine.HeaderAttribute;
  using SpaceAttribute = UnityEngine.SpaceAttribute;
  using RangeAttribute = UnityEngine.RangeAttribute;
  using HideInInspectorAttribute = UnityEngine.HideInInspector;
  using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;
  using FormerlySerializedAsAttribute = UnityEngine.Serialization.FormerlySerializedAsAttribute;
  using MovedFromAttribute = UnityEngine.Scripting.APIUpdating.MovedFromAttribute;
  using CreateAssetMenu = UnityEngine.CreateAssetMenuAttribute;
  using RuntimeInitializeOnLoadMethodAttribute = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;
  #endif //;
  
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.AIDriver))]
  public unsafe partial class AIDriverPrototype : ComponentPrototype<Quantum.AIDriver> {
    public SByte AIIndex;
    public FPVector3 TargetLocation;
    public FPVector3 NextTargetLocation;
    public AssetRef<AIDriverSettings> SettingsRef;
    public FP StationaryTime;
    partial void MaterializeUser(Frame frame, ref Quantum.AIDriver result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.AIDriver component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.AIDriver result, in PrototypeMaterializationContext context = default) {
        result.AIIndex = this.AIIndex;
        result.TargetLocation = this.TargetLocation;
        result.NextTargetLocation = this.NextTargetLocation;
        result.SettingsRef = this.SettingsRef;
        result.StationaryTime = this.StationaryTime;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Carrot))]
  public unsafe partial class CarrotPrototype : ComponentPrototype<Quantum.Carrot> {
    public AssetRef<CarrotAsset> Asset;
    public QBoolean PickedUp;
    partial void MaterializeUser(Frame frame, ref Quantum.Carrot result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Carrot component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Carrot result, in PrototypeMaterializationContext context = default) {
        result.Asset = this.Asset;
        result.PickedUp = this.PickedUp;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Checkpoint))]
  public unsafe partial class CheckpointPrototype : ComponentPrototype<Quantum.Checkpoint> {
    public SByte Index;
    public QBoolean Finish;
    public FPVector3 AIOptimalLocalPosition;
    public FPVector3 AIBadLocalPosition;
    partial void MaterializeUser(Frame frame, ref Quantum.Checkpoint result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Checkpoint component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Checkpoint result, in PrototypeMaterializationContext context = default) {
        result.Index = this.Index;
        result.Finish = this.Finish;
        result.AIOptimalLocalPosition = this.AIOptimalLocalPosition;
        result.AIBadLocalPosition = this.AIBadLocalPosition;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.DriftBoost))]
  public unsafe partial class DriftBoostPrototype : ComponentPrototype<Quantum.DriftBoost> {
    [HideInInspector()]
    public Int32 _empty_prototype_dummy_field_;
    partial void MaterializeUser(Frame frame, ref Quantum.DriftBoost result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.DriftBoost component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.DriftBoost result, in PrototypeMaterializationContext context = default) {
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.DriftBoostLevel))]
  public unsafe partial class DriftBoostLevelPrototype : StructPrototype {
    public FP MinDriftTime;
    public AssetRef<BoostConfig> BoostAsset;
    partial void MaterializeUser(Frame frame, ref Quantum.DriftBoostLevel result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.DriftBoostLevel result, in PrototypeMaterializationContext context = default) {
        result.MinDriftTime = this.MinDriftTime;
        result.BoostAsset = this.BoostAsset;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Drifting))]
  public unsafe partial class DriftingPrototype : ComponentPrototype<Quantum.Drifting> {
    public FP SideAcceleration;
    public FP ForwardFactor;
    public FP MaxSteeringOffset;
    public FP MinimumSpeed;
    public FP MaxAirTime;
    public FP MaxNoSteerTime;
    public FP MinSidewaysSpeedSqr;
    public FP MaxOppositeSteerTime;
    partial void MaterializeUser(Frame frame, ref Quantum.Drifting result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Drifting component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Drifting result, in PrototypeMaterializationContext context = default) {
        result.SideAcceleration = this.SideAcceleration;
        result.ForwardFactor = this.ForwardFactor;
        result.MaxSteeringOffset = this.MaxSteeringOffset;
        result.MinimumSpeed = this.MinimumSpeed;
        result.MaxAirTime = this.MaxAirTime;
        result.MaxNoSteerTime = this.MaxNoSteerTime;
        result.MinSidewaysSpeedSqr = this.MinSidewaysSpeedSqr;
        result.MaxOppositeSteerTime = this.MaxOppositeSteerTime;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Hazard))]
  public unsafe partial class HazardPrototype : ComponentPrototype<Quantum.Hazard> {
    public FP TimeAlive;
    public FP DamageRadius;
    public FP HitDuration;
    partial void MaterializeUser(Frame frame, ref Quantum.Hazard result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Hazard component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Hazard result, in PrototypeMaterializationContext context = default) {
        result.TimeAlive = this.TimeAlive;
        result.DamageRadius = this.DamageRadius;
        result.HitDuration = this.HitDuration;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Input))]
  public unsafe partial class InputPrototype : StructPrototype {
    public Button Drift;
    public Button Powerup;
    public Button Respawn;
    public Byte Encoded;
    partial void MaterializeUser(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context = default) {
        result.Drift = this.Drift;
        result.Powerup = this.Powerup;
        result.Respawn = this.Respawn;
        result.Encoded = this.Encoded;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Kart))]
  public unsafe partial class KartPrototype : ComponentPrototype<Quantum.Kart> {
    public AssetRef<KartStats> StatsAsset;
    public AssetRef<KartVisuals> VisualAsset;
    partial void MaterializeUser(Frame frame, ref Quantum.Kart result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Kart component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Kart result, in PrototypeMaterializationContext context = default) {
        result.StatsAsset = this.StatsAsset;
        result.VisualAsset = this.VisualAsset;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.KartBoost))]
  public unsafe partial class KartBoostPrototype : ComponentPrototype<Quantum.KartBoost> {
    [HideInInspector()]
    public Int32 _empty_prototype_dummy_field_;
    partial void MaterializeUser(Frame frame, ref Quantum.KartBoost result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.KartBoost component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.KartBoost result, in PrototypeMaterializationContext context = default) {
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.KartHitReceiver))]
  public unsafe partial class KartHitReceiverPrototype : ComponentPrototype<Quantum.KartHitReceiver> {
    public FP HitCooldown;
    public FPVector3 HitShapeSize;
    public LayerMask HazardLayerMask;
    partial void MaterializeUser(Frame frame, ref Quantum.KartHitReceiver result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.KartHitReceiver component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.KartHitReceiver result, in PrototypeMaterializationContext context = default) {
        result.HitCooldown = this.HitCooldown;
        result.HitShapeSize = this.HitShapeSize;
        result.HazardLayerMask = this.HazardLayerMask;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.KartInput))]
  public unsafe partial class KartInputPrototype : ComponentPrototype<Quantum.KartInput> {
    [HideInInspector()]
    public Int32 _empty_prototype_dummy_field_;
    partial void MaterializeUser(Frame frame, ref Quantum.KartInput result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.KartInput component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.KartInput result, in PrototypeMaterializationContext context = default) {
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.PlayerLink))]
  public unsafe partial class PlayerLinkPrototype : ComponentPrototype<Quantum.PlayerLink> {
    public PlayerRef Player;
    partial void MaterializeUser(Frame frame, ref Quantum.PlayerLink result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.PlayerLink component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.PlayerLink result, in PrototypeMaterializationContext context = default) {
        result.Player = this.Player;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Race))]
  public unsafe partial class RacePrototype : ComponentPrototype<Quantum.Race> {
    public SByte PositionCalcInterval;
    partial void MaterializeUser(Frame frame, ref Quantum.Race result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Race component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Race result, in PrototypeMaterializationContext context = default) {
        result.PositionCalcInterval = this.PositionCalcInterval;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.RaceProgress))]
  public unsafe partial class RaceProgressPrototype : ComponentPrototype<Quantum.RaceProgress> {
    [HideInInspector()]
    public Int32 _empty_prototype_dummy_field_;
    partial void MaterializeUser(Frame frame, ref Quantum.RaceProgress result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.RaceProgress component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.RaceProgress result, in PrototypeMaterializationContext context = default) {
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.RaceTrack))]
  public unsafe class RaceTrackPrototype : ComponentPrototype<Quantum.RaceTrack> {
    [DynamicCollectionAttribute()]
    public MapEntityId[] StartPositions = {};
    [DynamicCollectionAttribute()]
    public MapEntityId[] Checkpoints = {};
    [DynamicCollectionAttribute()]
    public MapEntityId[] Pickups = {};
    public SByte TotalLaps;
    public AssetRef<NavMesh> NavMesh;
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.RaceTrack component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.RaceTrack result, in PrototypeMaterializationContext context = default) {
        if (this.StartPositions.Length == 0) {
          result.StartPositions = default;
        } else {
          var list = frame.AllocateList(out result.StartPositions, this.StartPositions.Length);
          for (int i = 0; i < this.StartPositions.Length; ++i) {
            EntityRef tmp = default;
            PrototypeValidator.FindMapEntity(this.StartPositions[i], in context, out tmp);
            list.Add(tmp);
          }
        }
        if (this.Checkpoints.Length == 0) {
          result.Checkpoints = default;
        } else {
          var list = frame.AllocateList(out result.Checkpoints, this.Checkpoints.Length);
          for (int i = 0; i < this.Checkpoints.Length; ++i) {
            EntityRef tmp = default;
            PrototypeValidator.FindMapEntity(this.Checkpoints[i], in context, out tmp);
            list.Add(tmp);
          }
        }
        if (this.Pickups.Length == 0) {
          result.Pickups = default;
        } else {
          var list = frame.AllocateList(out result.Pickups, this.Pickups.Length);
          for (int i = 0; i < this.Pickups.Length; ++i) {
            EntityRef tmp = default;
            PrototypeValidator.FindMapEntity(this.Pickups[i], in context, out tmp);
            list.Add(tmp);
          }
        }
        result.TotalLaps = this.TotalLaps;
        result.NavMesh = this.NavMesh;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.RespawnMover))]
  public unsafe partial class RespawnMoverPrototype : ComponentPrototype<Quantum.RespawnMover> {
    public FP Progress;
    partial void MaterializeUser(Frame frame, ref Quantum.RespawnMover result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.RespawnMover component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.RespawnMover result, in PrototypeMaterializationContext context = default) {
        result.Progress = this.Progress;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.WheelConfig))]
  public unsafe partial class WheelConfigPrototype : StructPrototype {
    public FPVector3 Position;
    public FP Height;
    partial void MaterializeUser(Frame frame, ref Quantum.WheelConfig result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.WheelConfig result, in PrototypeMaterializationContext context = default) {
        result.Position = this.Position;
        result.Height = this.Height;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.WheelStatus))]
  public unsafe partial class WheelStatusPrototype : StructPrototype {
    public FPVector3 HitPoint;
    public FPVector3 HitNormal;
    public QBoolean Grounded;
    public AssetRef<DrivingSurface> HitSurface;
    partial void MaterializeUser(Frame frame, ref Quantum.WheelStatus result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.WheelStatus result, in PrototypeMaterializationContext context = default) {
        result.HitPoint = this.HitPoint;
        result.HitNormal = this.HitNormal;
        result.Grounded = this.Grounded;
        result.HitSurface = this.HitSurface;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Wheels))]
  public unsafe partial class WheelsPrototype : ComponentPrototype<Quantum.Wheels> {
    [ArrayLengthAttribute(4)]
    public Quantum.Prototypes.WheelConfigPrototype[] Configs = new Quantum.Prototypes.WheelConfigPrototype[4];
    public FP GroundedMinimumYFactor;
    public LayerMask DriveableMask;
    partial void MaterializeUser(Frame frame, ref Quantum.Wheels result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Wheels component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Wheels result, in PrototypeMaterializationContext context = default) {
        for (int i = 0, count = PrototypeValidator.CheckLength(Configs, 4, in context); i < count; ++i) {
          this.Configs[i].Materialize(frame, ref *result.Configs.GetPointer(i), in context);
        }
        result.GroundedMinimumYFactor = this.GroundedMinimumYFactor;
        result.DriveableMask = this.DriveableMask;
        MaterializeUser(frame, ref result, in context);
    }
  }
}
#pragma warning restore 0109
#pragma warning restore 1591
