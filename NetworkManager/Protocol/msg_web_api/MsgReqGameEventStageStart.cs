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
  public partial class MsgReqGameEventStageStart : TBase
  {
    private long _uid;
    private int _i_ModeId;
    private int _i_Difficulty;
    private int _i_StageId;
    private int _i_IsSweep;

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

    public int I_ModeId
    {
      get
      {
        return _i_ModeId;
      }
      set
      {
        __isset.i_ModeId = true;
        this._i_ModeId = value;
      }
    }

    public int I_Difficulty
    {
      get
      {
        return _i_Difficulty;
      }
      set
      {
        __isset.i_Difficulty = true;
        this._i_Difficulty = value;
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

    public int I_IsSweep
    {
      get
      {
        return _i_IsSweep;
      }
      set
      {
        __isset.i_IsSweep = true;
        this._i_IsSweep = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uid;
      public bool i_ModeId;
      public bool i_Difficulty;
      public bool i_StageId;
      public bool i_IsSweep;
    }

    public MsgReqGameEventStageStart() {
      this._i_ModeId = 0;
      this.__isset.i_ModeId = true;
      this._i_Difficulty = 0;
      this.__isset.i_Difficulty = true;
      this._i_StageId = 0;
      this.__isset.i_StageId = true;
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
              I_ModeId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_Difficulty = iprot.ReadI32();
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
              I_IsSweep = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgReqGameEventStageStart");
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
      if (__isset.i_ModeId) {
        field.Name = "i_ModeId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ModeId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_Difficulty) {
        field.Name = "i_Difficulty";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Difficulty);
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
      if (__isset.i_IsSweep) {
        field.Name = "i_IsSweep";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_IsSweep);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgReqGameEventStageStart(");
      sb.Append("Uid: ");
      sb.Append(Uid);
      sb.Append(",I_ModeId: ");
      sb.Append(I_ModeId);
      sb.Append(",I_Difficulty: ");
      sb.Append(I_Difficulty);
      sb.Append(",I_StageId: ");
      sb.Append(I_StageId);
      sb.Append(",I_IsSweep: ");
      sb.Append(I_IsSweep);
      sb.Append(")");
      return sb.ToString();
    }

  }

}