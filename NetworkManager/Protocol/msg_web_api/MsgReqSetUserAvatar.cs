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
  public partial class MsgReqSetUserAvatar : TBase
  {
    private long _uid;
    private int _i_AvatarId;

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

    public int I_AvatarId
    {
      get
      {
        return _i_AvatarId;
      }
      set
      {
        __isset.i_AvatarId = true;
        this._i_AvatarId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uid;
      public bool i_AvatarId;
    }

    public MsgReqSetUserAvatar() {
      this._i_AvatarId = 0;
      this.__isset.i_AvatarId = true;
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
              I_AvatarId = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgReqSetUserAvatar");
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
      if (__isset.i_AvatarId) {
        field.Name = "i_AvatarId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AvatarId);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgReqSetUserAvatar(");
      sb.Append("Uid: ");
      sb.Append(Uid);
      sb.Append(",I_AvatarId: ");
      sb.Append(I_AvatarId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}