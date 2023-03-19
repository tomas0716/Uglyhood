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
  public partial class MsgReqLogin : TBase
  {
    private MembershipInfoDID _membership_info_did;
    private string _s_AppVersion;

    public MembershipInfoDID Membership_info_did
    {
      get
      {
        return _membership_info_did;
      }
      set
      {
        __isset.membership_info_did = true;
        this._membership_info_did = value;
      }
    }

    public string S_AppVersion
    {
      get
      {
        return _s_AppVersion;
      }
      set
      {
        __isset.s_AppVersion = true;
        this._s_AppVersion = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool membership_info_did;
      public bool s_AppVersion;
    }

    public MsgReqLogin() {
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
            if (field.Type == TType.Struct) {
              Membership_info_did = new MembershipInfoDID();
              Membership_info_did.Read(iprot);
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.String) {
              S_AppVersion = iprot.ReadString();
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
      TStruct struc = new TStruct("MsgReqLogin");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Membership_info_did != null && __isset.membership_info_did) {
        field.Name = "membership_info_did";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        Membership_info_did.Write(oprot);
        oprot.WriteFieldEnd();
      }
      if (S_AppVersion != null && __isset.s_AppVersion) {
        field.Name = "s_AppVersion";
        field.Type = TType.String;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_AppVersion);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgReqLogin(");
      sb.Append("Membership_info_did: ");
      sb.Append(Membership_info_did== null ? "<null>" : Membership_info_did.ToString());
      sb.Append(",S_AppVersion: ");
      sb.Append(S_AppVersion);
      sb.Append(")");
      return sb.ToString();
    }

  }

}