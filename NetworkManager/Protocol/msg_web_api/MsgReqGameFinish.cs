﻿/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace sg.protocol.msg_web_api
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class MsgReqGameFinish : TBase
  {
    private long _uid;
    private int _i_EpisodeId;
    private int _i_ChapterId;
    private int _i_StageId;
    private int _i_IsClear;
    private int _i_StarCount;
    private int _i_DestroyColonyCount;
    private int _i_MaxComboCount;

    public long Uid
    {
      get
      {
        return _uid;
      }
      set
      {
        __isset.uid = true;
        this._uid = value;
      }
    }

    public int I_EpisodeId
    {
      get
      {
        return _i_EpisodeId;
      }
      set
      {
        __isset.i_EpisodeId = true;
        this._i_EpisodeId = value;
      }
    }

    public int I_ChapterId
    {
      get
      {
        return _i_ChapterId;
      }
      set
      {
        __isset.i_ChapterId = true;
        this._i_ChapterId = value;
      }
    }

    public int I_StageId
    {
      get
      {
        return _i_StageId;
      }
      set
      {
        __isset.i_StageId = true;
        this._i_StageId = value;
      }
    }

    public int I_IsClear
    {
      get
      {
        return _i_IsClear;
      }
      set
      {
        __isset.i_IsClear = true;
        this._i_IsClear = value;
      }
    }

    public int I_StarCount
    {
      get
      {
        return _i_StarCount;
      }
      set
      {
        __isset.i_StarCount = true;
        this._i_StarCount = value;
      }
    }

    public int I_DestroyColonyCount
    {
      get
      {
        return _i_DestroyColonyCount;
      }
      set
      {
        __isset.i_DestroyColonyCount = true;
        this._i_DestroyColonyCount = value;
      }
    }

    public int I_MaxComboCount
    {
      get
      {
        return _i_MaxComboCount;
      }
      set
      {
        __isset.i_MaxComboCount = true;
        this._i_MaxComboCount = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uid;
      public bool i_EpisodeId;
      public bool i_ChapterId;
      public bool i_StageId;
      public bool i_IsClear;
      public bool i_StarCount;
      public bool i_DestroyColonyCount;
      public bool i_MaxComboCount;
    }

    public MsgReqGameFinish() {
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.I64) {
              Uid = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_EpisodeId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_ChapterId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_StageId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              I_IsClear = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I32) {
              I_StarCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              I_DestroyColonyCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.I32) {
              I_MaxComboCount = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("MsgReqGameFinish");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.uid) {
        field.Name = "uid";
        field.Type = TType.I64;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(Uid);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_EpisodeId) {
        field.Name = "i_EpisodeId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_EpisodeId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ChapterId) {
        field.Name = "i_ChapterId";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ChapterId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_StageId) {
        field.Name = "i_StageId";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_StageId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_IsClear) {
        field.Name = "i_IsClear";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_IsClear);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_StarCount) {
        field.Name = "i_StarCount";
        field.Type = TType.I32;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_StarCount);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_DestroyColonyCount) {
        field.Name = "i_DestroyColonyCount";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_DestroyColonyCount);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_MaxComboCount) {
        field.Name = "i_MaxComboCount";
        field.Type = TType.I32;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_MaxComboCount);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgReqGameFinish(");
      sb.Append("Uid: ");
      sb.Append(Uid);
      sb.Append(",I_EpisodeId: ");
      sb.Append(I_EpisodeId);
      sb.Append(",I_ChapterId: ");
      sb.Append(I_ChapterId);
      sb.Append(",I_StageId: ");
      sb.Append(I_StageId);
      sb.Append(",I_IsClear: ");
      sb.Append(I_IsClear);
      sb.Append(",I_StarCount: ");
      sb.Append(I_StarCount);
      sb.Append(",I_DestroyColonyCount: ");
      sb.Append(I_DestroyColonyCount);
      sb.Append(",I_MaxComboCount: ");
      sb.Append(I_MaxComboCount);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
