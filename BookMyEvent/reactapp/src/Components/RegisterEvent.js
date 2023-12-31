import React, { useEffect, useState } from "react";
import {
  TextField,
  Select,
  MenuItem,
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  Button,
} from "@mui/material";
import OrganiserFormServices from "../Services/OrganiserFormServices";
import { json, useNavigate, useParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import store from "../App/store";
import EventServices from "../Services/EventServices";
import UserInputFormServices from "../Services/UserInputFormServices";
import Transactions from "./Transactions";
import { toast } from "react-toastify";
import { AddRegisteredEventId } from "../Features/ReducerSlices/HomeEventsSlice";
import { UpdateAvailableSeats } from "../Features/ReducerSlices/HomeEventsSlice";
import { CoPresentOutlined } from "@mui/icons-material";
import { getFileTypesThunk, getFormFieldsThunk } from "../Features/ReducerSlices/FormFieldsSlice";

const RegisterEvent = () => {
  const [eventRegistrationFormFields, setEventRegistrationFormFields] =
    useState([]);
  const [event, setEvent] = useState();
  const [toggleRegistrationTransaction, setToggleRegistrationTransaction] =
    useState(true);
  const [RegisteredData, setRegisteredData] = useState();
  const dispatch = useDispatch();
  let fetchedFormFields = [];
  const { eventId, formId } = useParams();
  const [TotalPrice, setTotalPrice] = useState(0);
  const [FormFieldTypes,setFormFieldTypes] = useState([]);
  const [FileTypes,setFileTypes] = useState([]);
  const [fieldRegistrationId, setFieldRegistrationId] = useState();
  useEffect(() => {
    const loadUserEventRegistrationFormFields = async () => {
      const event = await EventServices().getEventById(eventId);
      setEvent(event);
      //console.log(event);
      const formFields = await OrganiserFormServices().getFieldTypesByFormId(
        formId
      )
      console.log(formFields);
      formFields.forEach((e) => {
        setFieldRegistrationId((prev) => {
          return { ...prev, [e.lable]: e.registrationFormFieldId };
        });
      });
      setEventRegistrationFormFields([
        ...eventRegistrationFormFields,
        [...formFields],
      ]);      
      if(store.getState().formFields.formFields.length != 0){
        setFormFieldTypes(store.getState().formFields.formFields);
      }else{
        await dispatch(getFormFieldsThunk()).unwrap();
        setFormFieldTypes(store.getState().formFields.formFields);
      }
      if(store.getState().formFields.fileTypes.length != 0){
        setFileTypes(store.getState().formFields.fileTypes);
      }else{
        await dispatch(getFileTypesThunk()).unwrap();
        setFileTypes(store.getState().formFields.fileTypes);
      }
      fetchedFormFields = [...formFields];
      //console.log(fetchedFormFields);
    };
    loadUserEventRegistrationFormFields();
  }, []);

  const [formData, setFormData] = useState([]);

  const handlechange = (event, index, label) => {
    let { name } = event.target;
    let value = event.target.files ? event.target.files[0] : event.target.value;
    setFormData((prevData) => {
      prevData[index] = { ...prevData[index], [name]: value };
      //console.log(prevData);
      return prevData;
    });
    //console.log(formData);
  };

  const fileToBase64 = (file) => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);

      reader.onload = (e) => {
        console.log(e.target.result);
        resolve(e.target.result.split(',')[1]);
      };

      reader.onerror = (error) => {
        reject(error);
      };
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    let formResult = [];
    console.log(formData);
    for (let e of formData) {
      let formFieldResponse = [];
      for (let i in e) {
        const type = eventRegistrationFormFields[0].find((u) => {
          console.log(u.lable, i);
          return u.lable == i;
        })?.fieldTypeId;

        if (type == 1 || type == 3 || type == 6 || type == 5) {
          formFieldResponse.push({
            label: i,
            stringResponse: `${e[i]}`,
            registrationFormFieldId: fieldRegistrationId[i],
          });
          if (i == "Ticket Prices") {
            setTotalPrice((prev) => prev + Number(e[i]));
            //console.log(TotalPrice);
          }
        } else if (type == 2) {
          formFieldResponse.push({
            label: i,
            numberResponse: e[i],
            registrationFormFieldId: fieldRegistrationId[i],
          });
        } else if (type == 4) {
          const date = new Date(e[i]);
          formFieldResponse.push({
            label: i,
            dateResponse: date,
            registrationFormFieldId: fieldRegistrationId[i],
          });
        } else if (type == 7) {
          let file = await fileToBase64(e[i]);
          formFieldResponse.push({
            label: i,
            fileResponse: file,
            fileTypeId: FileTypes.find(a => a.fileTypeName == e[i].type)?.fileTypeId,
            registrationFormFieldId: fieldRegistrationId[i],
          });
          console.log(formFieldResponse);
        }
      }
      formResult.push(formFieldResponse);
    }
    console.log(formResult);
    let formResultPost = [];
    for (let i = 0; i < eventRegistrationFormFields.length; i++) {
      formResultPost.push({
        ["userInputFormBL"]: {
          ["userId"]: store.getState().auth.id,
          ["eventId"]: event.eventId,
        },
        ["userInputFormFields"]: formResult[i],
      });
    }
    setRegisteredData(
      await UserInputFormServices().submitUserInputForm(formResultPost)
    );
    dispatch(AddRegisteredEventId(event.eventId));
    dispatch(
      UpdateAvailableSeats({
        eventId: eventId,
        availableSeats: eventRegistrationFormFields.length,
      })
    );
    setToggleRegistrationTransaction(false);
  };
  const handleAddPerson = () => {
    if (
      Number.parseInt(
        event.availableSeats == -1 ? event.capacity : event.availableSeats
      ) > Number.parseInt(eventRegistrationFormFields.length)
    ) {
      if (
        eventRegistrationFormFields.length < event.maxNoOfTicketsPerTransaction
      ) {
        setEventRegistrationFormFields([
          ...eventRegistrationFormFields,
          [...eventRegistrationFormFields[0]],
        ]);
        //console.log(eventRegistrationFormFields);
      } else {
        toast.warning(
          `Per transaction you can register max of ${event.maxNoOfTicketsPerTransaction} tickets only`
        );
      }
    } else {
      toast.warning(`No seats Available`);
    }
  };
  const handleRemove = (index) => {
    if (eventRegistrationFormFields.length > 1) {
      const copyArray = [...eventRegistrationFormFields];
      copyArray.splice(index, 1);
      setEventRegistrationFormFields(copyArray);
      //-------------------------------------
      const copyFormData = [...formData];
      copyFormData.splice(index, 1);
      setFormData([...copyFormData]);
    }
  };
  const formContainerStyle = {
    margin: "0px auto",
    display: "flex",
    width: "90%",
    maxWidth: "800px",
    flexDirection: "column",
    alignItems: "center",
    padding: "20px",
    backgroundColor: "#fff",
    borderRadius: "8px",
    transition: "transform 0.3s ease",
    "&:hover": {
      transform: "scale(1.02)",
    },
  };
  let cnt = 0;
  const submitButtonStyle = {
    marginTop: "20px",
    backgroundColor: "#4caf50",
    color: "#fff",
    fontWeight: "bold",
    borderRadius: "4px",
    padding: "12px 24px",
    cursor: "pointer",
    border: "none",
    boxShadow: "0 2px 4px rgba(0, 0, 0, 0.2)",
    transition: "background-color 0.3s ease",
    textTransform: "uppercase",
    letterSpacing: "1px",
    fontSize: "14px",
    "&:hover": {
      backgroundColor: "#45a049",
    },
  };
  return (
    <>
      {toggleRegistrationTransaction ? (

        <form style={formContainerStyle} onSubmit={handleSubmit}>
          {eventRegistrationFormFields.map((person, index) => (

            <div
              key={cnt++}
              style={{
                padding: "30px",
                width: "100%",
                maxWidth: "800px",
                border: "1px solid #d0d0d0",
                borderRadius: "4px",
                marginBottom: "20px",
              }}
            >
              {person?.map((formField) => (
                <div key={cnt++}>
                  {FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "Text" ? (
                    <TextField
                      style={{
                        marginBottom: "20px",
                        width: "100%",
                        maxWidth: "800px",
                      }}
                      type="text"
                      name={formField.lable}
                      label={formField.lable}
                      value={formData[index]?.[formField.lable]}
                      onChange={(e) => {
                        handlechange(e, index, formField.lable);
                      }}
                      required
                    />
                  ) : FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "Number" ? (
                    <TextField
                      style={{
                        marginBottom: "20px",
                        width: "100%",
                        maxWidth: "800px",
                      }}
                      type="number"
                      name={formField.lable}
                      label={formField.lable}
                      value={formData[index]?.[formField.lable]}
                      onChange={(e) => {
                        handlechange(e, index);
                      }}
                      inputProps={{
                        min: Number(JSON.parse(formField.validations).min),
                        max: Number(JSON.parse(formField.validations).max),
                      }}
                      required
                    />
                  ) : FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "File" ? (
                    <TextField
                      style={{
                        marginBottom: "20px",
                        width: "100%",
                        maxWidth: "800px",
                      }}
                      type="file"
                      inputProps={{ accept: FileTypes.find(e => e.fileTypeId == Number(formField.fileTypeId))?.fileTypeName }}
                      name={formField.lable}
                      label={formField.lable}
                      value={formData[index]?.[formField.lable]}
                      onChange={(e) => {
                        handlechange(e, index);
                      }}
                      required
                    />
                  ) : FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "Email" ? (
                    <TextField
                      style={{
                        marginBottom: "20px",
                        width: "100%",
                        maxWidth: "800px ",
                      }}
                      type="email"
                      name={formField.lable}
                      label={formField.lable}
                      value={formData[index]?.[formField.lable]}
                      onChange={(e) => {
                        handlechange(e, index);
                      }}
                      required
                    />
                  ) : FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "Date" ? (
                    <TextField
                      style={{
                        marginBottom: "20px",
                        width: "100%",
                        maxWidth: "800px ",
                      }}
                      type="date"
                      name={formField.lable}
                      label={formField.lable}
                      value={formData[index]?.[formField.lable]}
                      onChange={(e) => {
                        handlechange(e, index);
                      }}
                      required
                    />
                  ) : FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "Radio" ? (
                    <FormControl
                      style={{
                        marginBottom: "20px",
                        width: "100%",
                        maxWidth: "800px ",
                      }}
                    >
                      <FormLabel>{formField.lable}</FormLabel>
                      <RadioGroup
                        name={formField.lable}
                        value={formData[index]?.[formField.lable]}
                        onChange={(e) => {
                          handlechange(e, index);
                        }}
                        required
                      >
                        {JSON.parse(formField.options).map((option) => (
                          <FormControlLabel
                            value={option}
                            control={<Radio />}
                            label={option}
                          />
                        ))}
                      </RadioGroup>
                    </FormControl>
                  ) : FormFieldTypes[Number(formField?.fieldTypeId) - 1]?.type ==
                    "Select" ? (
                    (event.isFree == false &&
                      formField.lable == "Ticket Prices") ||
                      formField.lable != "Ticket Prices" ? (
                      <FormControl
                        style={{
                          marginBottom: "20px",
                          width: "100%",
                          maxWidth: "800px ",
                        }}
                      >
                        <FormLabel>{formField.lable}</FormLabel>
                        <Select
                          name={formField.lable}
                          value={formData[index]?.[formField.lable]}
                          onChange={(e) => {
                            handlechange(e, index);
                          }}
                          required
                        >
                          {JSON.parse(formField.options).map((option) => (
                            <MenuItem key={cnt++} value={option}>
                              {option}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    ) : (
                      <></>
                    )
                  ) : (
                    <></>
                  )}
                </div>
              ))}
              <Button
                type="button"
                onClick={() => {
                  handleRemove(index);
                }}
              >
                remove
              </Button>
            </div>
          ))}
          <Button type="button" onClick={handleAddPerson}>
            Add Person
          </Button>

          <Button
            style={{
              marginBottom: "20px",
              width: "100%",
              maxWidth: "800px",
            }}
            type="submit"
            variant="contained"
            color="primary"
          >
            Register
          </Button>
        </form>
      ) : (
        <Transactions
          transactionData={{
            event: event,
            TotalPrice: TotalPrice,
            NoOfTickets: eventRegistrationFormFields.length,
            RegisteredData,
          }}
        />
      )}
    </>
  );
};

export default RegisterEvent;
