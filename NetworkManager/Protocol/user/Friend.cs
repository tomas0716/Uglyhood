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
  public partial class Friend : TBase
  {
    private long _i_FriendUin;
    private string _s_SocialUid;
    private string _s_Profile;
    private long _i_LastGiftTime;
    private short _i_FriendStatus;
    private bool _b_IsNew;
    private int _i_FriendKind;
    private long _i_LastLoginTime;
    private int _i_FriendTeamPoint;
    private string _s_FriendTeamName;
    private string _s_FriendTeamEmblem;

    public long I_FriendUin
    {
      get
      {
        return _i_FriendUin;
      }
      set
      {
        __isset.i_FriendUin = true;
        this._i_FriendUin = value;
      }
    }

    public string S_SocialUid
    {
      get
      {
        return _s_SocialUid;
      }
      set
      {
        __isset.s_SocialUid = true;
        this._s_SocialUid = value;
      }
    }

    public string S_Profile
    {
      get
      {
        return _s_Profile;
      }
      set
      {
        __isset.s_Profile = true;
        this._s_Profile = value;
      }
    }

    public long I_LastGiftTime
    {
      get
      {
        return _i_LastGiftTime;
      }
      set
      {
        __isset.i_LastGiftTime = true;
        this._i_LastGiftTime = value;
      }
    }

    public short I_FriendStatus
    {
      get
      {
        return _i_FriendStatus;
      }
      set
      {
        __isset.i_FriendStatus = true;
        this._i_FriendStatus = value;
      }
    }

    public bool B_IsNew
    {
      get
      {
        return _b_IsNew;
      }
      set
      {
        __isset.b_IsNew = true;
        this._b_IsNew = value;
      }
    }

    public int I_FriendKind
    {
      get
      {
        return _i_FriendKind;
      }
      set
      {
        __isset.i_FriendKind = true;
        this._i_FriendKind = value;
      }
    }

    public long I_LastLoginTime
    {
      get
      {
        return _i_LastLoginTime;
      }
      set
      {
        __isset.i_LastLoginTime = true;
        this._i_LastLoginTime = value;
      }
    }

    public int I_FriendTeamPoint
    {
      get
      {
        return _i_FriendTeamPoint;
      }
      set
      {
        __isset.i_FriendTeamPoint = true;
        this._i_FriendTeamPoint = value;
      }
    }

    public string S_FriendTeamName
    {
      get
      {
        return _s_FriendTeamName;
      }
      set
      {
        __isset.s_FriendTeamName = true;
        this._s_FriendTeamName = value;
      }
    }

    public string S_FriendTeamEmblem
    {
      get
      {
        return _s_FriendTeamEmblem;
      }
      set
      {
        __isset.s_FriendTeamEmblem = true;
        this._s_FriendTeamEmblem = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_FriendUin;
      public bool s_SocialUid;
      public bool s_Profile;
      public bool i_LastGiftTime;
      public bool i_FriendStatus;
      public bool b_IsNew;
      public bool i_FriendKind;
      public bool i_LastLoginTime;
      public bool i_FriendTeamPoint;
      public bool s_FriendTeamName;
      public bool s_FriendTeamEmblem;
    }

    public Friend() {
      this._i_FriendUin = 0;
      this.__isset.i_FriendUin = true;
      this._s_SocialUid = "";
      this.__isset.s_SocialUid = true;
      this._s_Profile = "";
      this.__isset.s_Profile = true;
      this._i_LastGiftTime = 0;
      this.__isset.i_LastGiftTime = true;
      this._i_FriendStatus = 0;
      this.__isset.i_FriendStatus = true;
      this._b_IsNew = false;
      this.__isset.b_IsNew = true;
      this._i_FriendKind = 0;
      this.__isset.i_FriendKind = true;
      this._i_LastLoginTime = 0;
      this.__isset.i_LastLoginTime = true;
      this._i_FriendTeamPoint = 0;
      this.__isset.i_FriendTeamPoint = true;
      this._s_FriendTeamName = "";
      this.__isset.s_FriendTeamName = true;
      this._s_FriendTeamEmblem = "";
      this.__isset.s_FriendTeamEmblem = true;
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
              I_FriendUin = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              S_SocialUid = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              S_Profile = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I64) {
              I_LastGiftTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I16) {
              I_FriendStatus = iprot.ReadI16();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.Bool) {
              B_IsNew = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 7:
            if (field.Type == TType.I32) {
              I_FriendKind = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 8:
            if (field.Type == TType.I64) {
              I_LastLoginTime = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 9:
            if (field.Type == TType.I32) {
              I_FriendTeamPoint = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 10:
            if (field.Type == TType.String) {
              S_FriendTeamName = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 11:
            if (field.Type == TType.String) {
              S_FriendTeamEmblem = iprot.ReadString();
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
      TStruct struc = new TStruct("Friend");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_FriendUin) {
        field.Name = "i_FriendUin";
        field.Type = TType.I64;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(I_FriendUin);
        oprot.WriteFieldEnd();
      }
      if (S_SocialUid != null && __isset.s_SocialUid) {
        field.Name = "s_SocialUid";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_SocialUid);
        oprot.WriteFieldEnd();
      }
      if (S_Profile != null && __isset.s_Profile) {
        field.Name = "s_Profile";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_Profile);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_LastGiftTime) {
        field.Name = "i_LastGiftTime";
        field.Type = TType.I64;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(I_LastGiftTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_FriendStatus) {
        field.Name = "i_FriendStatus";
        field.Type = TType.I16;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI16(I_FriendStatus);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsNew) {
        field.Name = "b_IsNew";
        field.Type = TType.Bool;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsNew);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_FriendKind) {
        field.Name = "i_FriendKind";
        field.Type = TType.I32;
        field.ID = 7;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_FriendKind);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_LastLoginTime) {
        field.Name = "i_LastLoginTime";
        field.Type = TType.I64;
        field.ID = 8;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(I_LastLoginTime);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_FriendTeamPoint) {
        field.Name = "i_FriendTeamPoint";
        field.Type = TType.I32;
        field.ID = 9;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_FriendTeamPoint);
        oprot.WriteFieldEnd();
      }
      if (S_FriendTeamName != null && __isset.s_FriendTeamName) {
        field.Name = "s_FriendTeamName";
        field.Type = TType.String;
        field.ID = 10;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_FriendTeamName);
        oprot.WriteFieldEnd();
      }
      if (S_FriendTeamEmblem != null && __isset.s_FriendTeamEmblem) {
        field.Name = "s_FriendTeamEmblem";
        field.Type = TType.String;
        field.ID = 11;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_FriendTeamEmblem);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Friend(");
      sb.Append("I_FriendUin: ");
      sb.Append(I_FriendUin);
      sb.Append(",S_SocialUid: ");
      sb.Append(S_SocialUid);
      sb.Append(",S_Profile: ");
      sb.Append(S_Profile);
      sb.Append(",I_LastGiftTime: ");
      sb.Append(I_LastGiftTime);
      sb.Append(",I_FriendStatus: ");
      sb.Append(I_FriendStatus);
      sb.Append(",B_IsNew: ");
      sb.Append(B_IsNew);
      sb.Append(",I_FriendKind: ");
      sb.Append(I_FriendKind);
      sb.Append(",I_LastLoginTime: ");
      sb.Append(I_LastLoginTime);
      sb.Append(",I_FriendTeamPoint: ");
      sb.Append(I_FriendTeamPoint);
      sb.Append(",S_FriendTeamName: ");
      sb.Append(S_FriendTeamName);
      sb.Append(",S_FriendTeamEmblem: ");
      sb.Append(S_FriendTeamEmblem);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
