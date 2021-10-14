import React, { useContext } from "react";
import {User} from "../../Api/apiClient";
import {UpdateUserRole} from "../../Api/apiClient";
import {Card} from "../Card/Card";
import "./UserCard.scss";
import {Link} from "react-router-dom";
import { LoginContext } from "../LoginManager/LoginManager";
// import { LoginContext } from "../LoginManager/LoginManager";

interface UserCardProps {
    user: User;
}

export function UserCard(props: UserCardProps) {
    const login = useContext(LoginContext);
    return (
        <Card>
            <Link className="user-card" to={`/users/${props.user.id}`}>
                <img className="profile-image" src={props.user.profileImageUrl} alt=""/>
                <div className="user-details">
                    <div className="name">{props.user.displayName}</div>
                    <div className="username">{props.user.username}</div>
                </div>
                <button type="submit"
                onClick={() => {
                    UpdateUserRole(props.user.id, login.username, login.password)}
                    }
                > Update Role </button>
            </Link>
        </Card>
    );
}