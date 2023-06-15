// import React, { useState } from "react";
// import { TextField, Button, Container, Stack } from "@mui/material";
// import { Link } from "react-router-dom";

// const RegisterForm = () => {
//   const [EventForm, SetEventForm] = useState({});

//   function handleSubmit(event) {
//     event.preventDefault();
//     console.log(firstName, lastName, email, dateOfBirth, password);
//   }
//   const handleOnChange = (e) => {
//     SetEventForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
//   };
//   return (
//     <React.Fragment>
//       <h2>Register Form</h2>
//       <form onSubmit={handleSubmit} action={<Link to="/login" />}>
//       <TextField
//           type="text"
//           variant="outlined"
//           color="secondary"
//           label="EventName"
//           name="EventName"
//           onChange={handleOnChange}
//           value={EventForm.EventName}
//           fullWidth
//           required
//           sx={{ mb: 4 }}
//         />
//         <TextField
//           type="text"
//           variant="outlined"
//           color="secondary"
//           label="Description"
//           name="Description"
//           onChange={handleOnChange}
//           value={EventForm.Description}
//           fullWidth
//           required
//           sx={{ mb: 4 }}
//         />
        
//         <Stack spacing={2} direction="row" sx={{ marginBottom: 4 }}>
//           <TextField
//             type="text"
//             variant="outlined"
//             color="secondary"
//             label="StartDate"
//             onChange={(e) => setFirstName(e.target.value)}
//             value={firstName}
//             fullWidth
//             required
//           />
//           <TextField
//             type="text"
//             variant="outlined"
//             color="secondary"
//             label="EndDate"
//             onChange={(e) => setLastName(e.target.value)}
//             value={lastName}
//             fullWidth
//             required
//           />
//         </Stack>
//         <TextField
//           type="email"
//           variant="outlined"
//           color="secondary"
//           label="Email"
//           onChange={(e) => setEmail(e.target.value)}
//           value={email}
//           fullWidth
//           required
//           sx={{ mb: 4 }}
//         />
//         <TextField
//           type="password"
//           variant="outlined"
//           color="secondary"
//           label="Password"
//           onChange={(e) => setPassword(e.target.value)}
//           value={password}
//           required
//           fullWidth
//           sx={{ mb: 4 }}
//         />
//         <TextField
//           type="date"
//           variant="outlined"
//           color="secondary"
//           label="Date of Birth"
//           onChange={(e) => setDateOfBirth(e.target.value)}
//           value={dateOfBirth}
//           fullWidth
//           required
//           sx={{ mb: 4 }}
//         />
//         <Button variant="outlined" color="secondary" type="submit">
//           Register
//         </Button>
//       </form>
//       <small>
//         Already have an account? <Link to="/login">Login Here</Link>
//       </small>
//     </React.Fragment>
//   );
// };

// export default RegisterForm;
