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

namespace sg.protocol.user
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class UserInfoBasic : TBase
  {
    private long _uid;
    private int _i_UserRank;
    private int _i_UserExp;
    private string _s_PlayerName;
    private int _i_UserAvatar;

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

    public int I_UserRank
    {
      get
      {
        return _i_UserRank;
      }
      set
      {
        __isset.i_UserRank = true;
        this._i_UserRank = value;
      }
    }

    public int I_UserExp
    {
      get
      {
        return _i_UserExp;
      }
      set
      {
        __isset.i_UserExp = true;
        this._i_UserExp = value;
      }
    }

    public string S_PlayerName
    {
      get
      {
        return _s_PlayerName;
      }
      set
      {
        __isset.s_PlayerName = true;
        this._s_PlayerName = value;
      }
    }

    public int I_UserAvatar
    {
      get
      {
        return _i_UserAvatar;
      }
      set
      {
        __isset.i_UserAvatar = true;
        this._i_UserAvatar = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool uid;
      public bool i_UserRank;
      public bool i_UserExp;
      public bool s_PlayerName;
      public bool i_UserAvatar;
    }

    public UserInfoBasic() {
      this._uid = 0;
      this.__isset.uid = true;
      this._i_UserRank = 0;
      this.__isset.i_UserRank = true;
      this._i_UserExp = 0;
      this.__isset.i_UserExp = true;
      this._i_UserAvatar = 0;
      this.__isset.i_UserAvatar = true;
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
              I_UserRank = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_UserExp = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              S_PlayerName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              I_UserAvatar = iprot.ReadI32();
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
      TStruct struc = new TStruct("UserInfoBasic");
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
      if (__isset.i_UserRank) {
        field.Name = "i_UserRank";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_UserRank);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_UserExp) {
        field.Name = "i_UserExp";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_UserExp);
        oprot.WriteFieldEnd();
      }
      if (S_PlayerName != null && __isset.s_PlayerName) {
        field.Name = "s_PlayerName";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_PlayerName);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_UserAvatar) {
        field.Name = "i_UserAvatar";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_UserAvatar);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("UserInfoBasic(");
      sb.Append("Uid: ");
      sb.Append(Uid);
      sb.Append(",I_UserRank: ");
      sb.Append(I_UserRank);
      sb.Append(",I_UserExp: ");
      sb.Append(I_UserExp);
      sb.Append(",S_PlayerName: ");
      sb.Append(S_PlayerName);
      sb.Append(",I_UserAvatar: ");
      sb.Append(I_UserAvatar);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
