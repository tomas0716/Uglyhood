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
  public partial class MsgReqCreateUser : TBase
  {
    private MembershipInfoDID _membership_info_did;
    private int _i_Platform;

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

    public int I_Platform
    {
      get
      {
        return _i_Platform;
      }
      set
      {
        __isset.i_Platform = true;
        this._i_Platform = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool membership_info_did;
      public bool i_Platform;
    }

    public MsgReqCreateUser() {
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
            if (field.Type == TType.I32) {
              I_Platform = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgReqCreateUser");
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
      if (__isset.i_Platform) {
        field.Name = "i_Platform";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Platform);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgReqCreateUser(");
      sb.Append("Membership_info_did: ");
      sb.Append(Membership_info_did== null ? "<null>" : Membership_info_did.ToString());
      sb.Append(",I_Platform: ");
      sb.Append(I_Platform);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
