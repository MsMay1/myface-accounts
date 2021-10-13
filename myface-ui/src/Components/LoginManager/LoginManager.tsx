﻿import React, { createContext, ReactNode, useState } from "react";

export const LoginContext = createContext({
    isLoggedIn: false,
    isAdmin: false,
    username: "",
    password: "",
    logIn: (username: string, password: string) => { },
    logOut: () => { },
    // authHeader: Headers
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    function logIn(username: string, password: string) {
        // authenticate if(details are correct){}
        setUsername(username);
        setPassword(password);
        setLoggedIn(true);

    }

    function logOut() {
        setLoggedIn(false);
        setUsername("");
        setPassword("");
    }

    const context = {
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        username: username,
        password: password,
        logIn: logIn,
        logOut: logOut,
        // authHeader: Headers
    };

    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}