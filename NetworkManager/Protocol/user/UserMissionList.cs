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
  public partial class UserMissionList : TBase
  {
    private List<UserMission> _l_MissionList;

    public List<UserMission> L_MissionList
    {
      get
      {
        return _l_MissionList;
      }
      set
      {
        __isset.l_MissionList = true;
        this._l_MissionList = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool l_MissionList;
    }

    public UserMissionList() {
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
            if (field.Type == TType.List) {
              {
                L_MissionList = new List<UserMission>();
                TList _list4 = iprot.ReadListBegin();
                for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                {
                  UserMission _elem6 = new UserMission();
                  _elem6 = new UserMission();
                  _elem6.Read(iprot);
                  L_MissionList.Add(_elem6);
                }
                iprot.ReadListEnd();
              }
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
      TStruct struc = new TStruct("UserMissionList");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (L_MissionList != null && __isset.l_MissionList) {
        field.Name = "l_MissionList";
        field.Type = TType.List;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, L_MissionList.Count));
          foreach (UserMission _iter7 in L_MissionList)
          {
            _iter7.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("UserMissionList(");
      sb.Append("L_MissionList: ");
      sb.Append(L_MissionList);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
