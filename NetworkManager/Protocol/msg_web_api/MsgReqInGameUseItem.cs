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
  public partial class MsgReqInGameUseItem : TBase
  {
    private long _uid;
    private int _i_EpisodeId;
    private int _i_ChapterId;
    private int _i_StageId;
    private int _i_Id;
    private int _i_Count;

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

    public int I_Id
    {
      get
      {
        return _i_Id;
      }
      set
      {
        __isset.i_Id = true;
        this._i_Id = value;
      }
    }

    public int I_Count
    {
      get
      {
        return _i_Count;
      }
      set
      {
        __isset.i_Count = true;
        this._i_Count = value;
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
      public bool i_Id;
      public bool i_Count;
    }

    public MsgReqInGameUseItem() {
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
              I_Id = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I32) {
              I_Count = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgReqInGameUseItem");
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
      if (__isset.i_Id) {
        field.Name = "i_Id";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Id);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_Count) {
        field.Name = "i_Count";
        field.Type = TType.I32;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Count);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgReqInGameUseItem(");
      sb.Append("Uid: ");
      sb.Append(Uid);
      sb.Append(",I_EpisodeId: ");
      sb.Append(I_EpisodeId);
      sb.Append(",I_ChapterId: ");
      sb.Append(I_ChapterId);
      sb.Append(",I_StageId: ");
      sb.Append(I_StageId);
      sb.Append(",I_Id: ");
      sb.Append(I_Id);
      sb.Append(",I_Count: ");
      sb.Append(I_Count);
      sb.Append(")");
      return sb.ToString();
    }

  }

}